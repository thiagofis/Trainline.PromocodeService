using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trainline.NetStandard.StandardHeaders.Services;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Host.Constants;
using Trainline.PromocodeService.Host.Mappers;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Host.Extensions;
using Trainline.PromocodeService.Service.Exceptions;
using Trainline.VortexPublisher.EventPublishing;
using Redeemed = Trainline.PromocodeService.Contract.Redeemed;
using Trainline.PromocodeService.Common.Exceptions;

namespace Trainline.PromocodeService.Host.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("promocodes")]
    public class PromocodeController : ControllerBase
    {
        private readonly IPromocodeService _promocodeService;
        private readonly IPromocodeValidator _promocodeValidator;
        private readonly IPromocodeMapper _promocodeMapper;
        private readonly IInvoiceMapper _invoiceMapper;
        private readonly IRedemptionMapper _redemptionMapper;
        private readonly IVortexEventPublisher _vortexEventPublisher;
        private readonly IHeaderService _headerService;
        private readonly IDateTimeProvider _dateTimeProvider;


        public PromocodeController(IPromocodeService promocodeService, IPromocodeValidator promocodeValidator, IPromocodeMapper promocodeMapper, IInvoiceMapper invoiceMapper,
            IRedemptionMapper redemptionMapper, IVortexEventPublisher vortexEventPublisher, IHeaderService headerService, IDateTimeProvider dateTimeProvider)
        {
            _promocodeService = promocodeService;
            _promocodeValidator = promocodeValidator;
            _promocodeMapper = promocodeMapper;
            _invoiceMapper = invoiceMapper;
            _redemptionMapper = redemptionMapper;
            _vortexEventPublisher = vortexEventPublisher;
            _headerService = headerService;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet("{promocodeId}", Name = RouteNames.GetPromocode)]
        public async Task<IActionResult> Get(string promocodeId)
        {
            var promocode = await _promocodeService.GetByPromocodeId(promocodeId);

            return promocode != null
                ? (IActionResult) Ok(_promocodeMapper.Map(promocode))
                : throw new PromocodeNotFoundException();
        }

        [HttpPost("", Name = RouteNames.CreatePromocode)]
        public async Task<CreatedResult> Create([FromBody] CreatePromocode createPromocode)
        {
            if (!createPromocode.Code.All(char.IsLetterOrDigit))
            {
                throw new InvalidPromocodeException($"{createPromocode.Code} is not a valid promocode. It must contain only alphanumeric characters.");
            }

            var promocode = await _promocodeService.Create(createPromocode?.Code);

            var contract = _promocodeMapper.Map(promocode);
            _vortexEventPublisher.NotifyVortexOfPromocodeCreated(promocode, _headerService);

            return Created(contract.Links["self"].Href, contract);
        }

        [HttpPost("{promocodeId}/apply", Name = RouteNames.ApplyPromocode)]
        public async Task<ICollection<Contract.DiscountItem>> Apply(string promocodeId, [FromBody] Invoice[] invoices)
        {
            await _promocodeValidator.Validate(promocodeId);

            invoices.ValidateCurrency();

            var invoiceInfos = await _invoiceMapper.Map(invoices);
            var applied = await _promocodeService.Apply(promocodeId, invoiceInfos);

            var discountedInvoices = applied.DiscountInvoiceInfo;
            var campaignName = applied.CampaignName;

            _vortexEventPublisher.NotifyVortexOfPromocodeValidated(promocodeId, discountedInvoices, _headerService, campaignName );
            return _invoiceMapper.MapDiscounts(discountedInvoices);
        }

        [HttpPost("{promocodeId}/redeem", Name = RouteNames.RedeemPromocode)]
        public async Task<Redeemed> Redeem(string promocodeId, [FromBody] Invoice[] invoices)
        {
            await _promocodeValidator.Validate(promocodeId);

            invoices.ValidateCurrency();

            var invoiceInfos = await _invoiceMapper.Map(invoices);
            var redemption = await _promocodeService.Redeem(promocodeId, invoiceInfos, _dateTimeProvider.DefaultRetentionDate);

            _vortexEventPublisher.NotifyVortexOfPromocodeRedeemed(promocodeId, redemption.InvoiceInfos, _headerService, redemption.CampaignName);
            return _redemptionMapper.Map(redemption);
        }
    }
}
