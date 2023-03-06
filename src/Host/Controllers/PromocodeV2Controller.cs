using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Trainline.NetStandard.StandardHeaders.Services;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Host.Constants;
using Trainline.PromocodeService.Host.Extensions;
using Trainline.PromocodeService.Host.Mappers;
using Trainline.PromocodeService.Service;
using Trainline.VortexPublisher.EventPublishing;

namespace Trainline.PromocodeService.Host.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("promocodes")]
    public class PromocodeV2Controller : ControllerBase
    {
        private readonly IPromocodeService _promocodeService;
        private readonly IPromocodeValidator _promocodeValidator;
        private readonly IInvoiceMapper _invoiceMapper;
        private readonly IRedemptionMapper _redemptionMapper;
        private readonly IVortexEventPublisher _vortexEventPublisher;
        private readonly IHeaderService _headerService;
        private readonly IDateTimeProvider _dateTimeProvider;


        public PromocodeV2Controller(IPromocodeService promocodeService, IPromocodeValidator promocodeValidator, IInvoiceMapper invoiceMapper,
            IRedemptionMapper redemptionMapper, IVortexEventPublisher vortexEventPublisher, IHeaderService headerService, IDateTimeProvider dateTimeProvider)
        {
            _promocodeService = promocodeService;
            _promocodeValidator = promocodeValidator;
            _invoiceMapper = invoiceMapper;
            _redemptionMapper = redemptionMapper;
            _vortexEventPublisher = vortexEventPublisher;
            _headerService = headerService;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpPost("{promocodeId}/redeem", Name = RouteNames.RedeemPromocode)]
        public async Task<Redeemed> RedeemV2(string promocodeId, [FromBody] RedeemPromocode redeemPromocode)
        {
            await _promocodeValidator.Validate(promocodeId);

            redeemPromocode.Invoices.ValidateCurrency();

            var invoiceInfos = await _invoiceMapper.Map(redeemPromocode.Invoices);

            var retentionDate = GetRetentionDate(redeemPromocode);
            var redemption = await _promocodeService.Redeem(promocodeId, invoiceInfos, retentionDate);

            _vortexEventPublisher.NotifyVortexOfPromocodeRedeemed(promocodeId, redemption.InvoiceInfos, _headerService, redemption.CampaignName);
            return _redemptionMapper.Map(redemption);
        }

        private DateTime GetRetentionDate(RedeemPromocode redeemPromocode)
        {
            return redeemPromocode.RetentionDate ?? _dateTimeProvider.DefaultRetentionDate;
        }
    }
}
