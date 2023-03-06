using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Trainline.NetStandard.StandardHeaders.Services;
using Trainline.PromocodeService.Common.Exceptions;
using Trainline.PromocodeService.ExternalServices.Voucherify;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service.Mappers;
using Trainline.PromocodeService.Service.Repository;
using Trainline.PromocodeService.Service.Repository.Entities;
using Trainline.PromocodeService.Service.Extensions;
using Promocode = Trainline.PromocodeService.Model.Promocode;
using Redeemed = Trainline.PromocodeService.Model.Redeemed;

namespace Trainline.PromocodeService.Service
{
    public class PromocodeService : IPromocodeService
    {
        private readonly IVoucherifyClient _voucherifyClient;
        private readonly IPromocodeRepository _promocodeRepository;
        private readonly IRedemptionRepository _redemptionRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IPromocodeMapper _promocodeMapper;
        private readonly IVoucherifyMapper _voucherifyMapper;
        private readonly IRedemptionMapper _redemptionMapper;
        private readonly IValidationRuleService _validationRuleService;
        private readonly IInvoiceGenerator _invoiceGenerator;
        private readonly ILedgerService _ledgerService;
        private readonly IHeaderService _headerService;

        private const int RetentionDateWhenCreatePromocodeInMonths = 6;
        private const int MaxRetentionDateInMonths = 43;

        public PromocodeService(IVoucherifyClient voucherifyClient,
            IPromocodeRepository promocodeRepository,
            IRedemptionRepository redemptionRepository,
            ICampaignRepository campaignRepository,
            IPromocodeMapper promocodeMapper,
            IVoucherifyMapper voucherifyMapper,
            IRedemptionMapper redemptionMapper,
            IInvoiceGenerator invoiceGenerator,
            IValidationRuleService validationRuleService,
            ILedgerService ledgerService,
            IHeaderService headerService)
        {
            _voucherifyClient = voucherifyClient;
            _promocodeRepository = promocodeRepository;
            _redemptionRepository = redemptionRepository;
            _campaignRepository = campaignRepository;
            _promocodeMapper = promocodeMapper;
            _voucherifyMapper = voucherifyMapper;
            _redemptionMapper = redemptionMapper;
            _invoiceGenerator = invoiceGenerator;
            _validationRuleService = validationRuleService;
            _ledgerService = ledgerService;
            _headerService = headerService;
        }

        public async Task<Promocode> Create(string code)
        {
            var promocodeEntity = await _promocodeRepository.GetByCode(code);

            if (promocodeEntity == null)
            {
                var voucher = await _voucherifyClient.GetVoucher(code);

                if (voucher.Active == false)
                {
                    throw new InvalidPromocodeException("Promocode is inactive.");
                }

                var campaign = await _campaignRepository.Get(voucher.CampaignId);

                if (campaign != null && campaign.Redeemable == false)
                {
                    throw new RedemptionTotalLimitReachedException("Campaign has reached maximum number of vouchers redeemed.");
                }

                if (campaign == null)
                {
                    try
                    {
                        await _campaignRepository.Add(new CampaignEntity {CampaignId = voucher.CampaignId});
                    }
                    catch (SqlException ex)
                    {
                        // If the campaign was already added, we are consuming the exception and carry on.
                    }
                }

                promocodeEntity = _promocodeMapper.Map(voucher);
                promocodeEntity.PromocodeId = Guid.NewGuid().ToString();
                promocodeEntity.RetentionDate = promocodeEntity.ValidityEndDate.AddMonths(RetentionDateWhenCreatePromocodeInMonths);
                promocodeEntity = await _promocodeRepository.Add(promocodeEntity);
            }
            else
            {
                await ValidateCampaignIdInPromocodeRepository(promocodeEntity);
            }

            var validationRules = await _validationRuleService.Get(promocodeEntity.ValidationRuleId);
            var promocode = _promocodeMapper.Map(promocodeEntity, validationRules);
            return promocode;
        }

        public async Task<Promocode> GetByPromocodeId(string promocodeId)
        {
            var promocodeEntity = await _promocodeRepository.GetByPromocodeId(promocodeId);
            if (promocodeEntity == null)
            {
                return null;
            }

            var validationRules = await _validationRuleService.Get(promocodeEntity.ValidationRuleId);
            return _promocodeMapper.Map(promocodeEntity, validationRules);
        }

        public async Task<Applied> Apply(string promocodeId, ICollection<InvoiceInfo> invoices)
        {
            var promocode = await _promocodeRepository.GetByPromocodeId(promocodeId);

            await ValidateCampaignIdInPromocodeRepository(promocode);
            var validation = await _voucherifyMapper.Map(invoices, _headerService.GetCustomerUri());

            Validated response;
            try
            {
                response = await _voucherifyClient.ValidateVoucher(promocode.Code, validation);
            }
            catch (RedemptionTotalLimitReachedException e)
            {
                var campaign = await _campaignRepository.Get(promocode.CampaignId);
                campaign.Redeemable = false;
                await _campaignRepository.Update(campaign);
                throw e;
            }

            return new Applied
            {
                CampaignName = promocode.CampaignName,
                DiscountInvoiceInfo = _invoiceGenerator.Generate(response, invoices).ToList()
            };
        }

        public async Task<Redeemed> Redeem(string promocodeId, ICollection<InvoiceInfo> invoices, DateTime retentionDate)
        {
            ExternalServices.Voucherify.Contract.Redeemed response;
            var promocode = await _promocodeRepository.GetByPromocodeId(promocodeId);

            await ValidateCampaignIdInPromocodeRepository(promocode);

            var redeem = await _voucherifyMapper.MapRedeem(invoices, _headerService.GetCustomerUri());

            try
            {
                response = await _voucherifyClient.RedeemVoucher(promocode.Code, redeem);
            }
            catch (RedemptionTotalLimitReachedException e)
            {
                var campaign = await _campaignRepository.Get(promocode.CampaignId);
                campaign.Redeemable = false;
                await _campaignRepository.Update(campaign);

                throw new RedemptionTotalLimitReachedException(
                    "Campaign has reached maximum number of vouchers redeemed.");
            }


            var redemptionEntity = _redemptionMapper.Map(response);
            redemptionEntity.PromocodeId = promocodeId;
            await _redemptionRepository.Add(redemptionEntity);

            var promocodeEntity = _promocodeMapper.Map(response.Voucher);
            promocodeEntity.ValidationRuleId = promocode.ValidationRuleId;
            promocodeEntity.PromocodeId = promocodeId;
            promocodeEntity.RetentionDate = promocode.RetentionDate > retentionDate ? promocode.RetentionDate : retentionDate;


            await _promocodeRepository.Update(promocodeEntity);

            var redeemed = new Redeemed
            {
                Id = response.Id,
                Code = promocode.Code,
                PromocodeId = promocode.PromocodeId,
                CampaignName = promocode.CampaignName,
                InvoiceInfos = _invoiceGenerator.Generate(response, invoices).ToList()
            };

            await _ledgerService.Create(redeemed);

            return redeemed;
        }

        private async Task ValidateCampaignIdInPromocodeRepository(Repository.Entities.Promocode promocode)
        {
            if (promocode.CampaignId == null)
            {
                var voucher = await _voucherifyClient.GetVoucher(promocode.Code);
                var campaign = await _campaignRepository.Get(voucher.CampaignId);
                if (campaign == null)
                {
                   await _campaignRepository.Add(new CampaignEntity { CampaignId = voucher.CampaignId });
                }

                promocode.CampaignId = voucher.CampaignId;
                await _promocodeRepository.Update(promocode);
            }
        }
    }
}
