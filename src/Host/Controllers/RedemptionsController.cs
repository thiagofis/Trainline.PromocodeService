using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trainline.NetStandard.StandardHeaders.Services;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Host.Constants;
using Trainline.PromocodeService.Host.Mappers;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Host.Extensions;
using Trainline.VortexPublisher.EventPublishing;

namespace Trainline.PromocodeService.Host.Controllers
{
    [ApiController]
    [Route("promocodes/{promocodeId}")]
    public class RedemptionsController : ControllerBase
    {
        private readonly IRedemptionService _redemptionService;
        private readonly IRedemptionMapper _redemptionMapper;
        private readonly ILedgerService _ledgerService;
        private readonly ILedgerMapper _ledgerMapper;
        private readonly IVortexEventPublisher _vortexEventPublisher;
        private readonly IHeaderService _headerService;

        public RedemptionsController(IRedemptionService redemptionService, IRedemptionMapper redemptionMapper, ILedgerService ledgerService,
            ILedgerMapper ledgerMapper, IVortexEventPublisher vortexEventPublisher, IHeaderService headerService)
        {
            _redemptionService = redemptionService;
            _redemptionMapper = redemptionMapper;
            _ledgerService = ledgerService;
            _ledgerMapper = ledgerMapper;
            _vortexEventPublisher = vortexEventPublisher;
            _headerService = headerService;
        }


        [HttpGet("redemptions", Name = RouteNames.GetAllRedemption)]
        public async Task<IActionResult> GetAllRedemption(string promocodeId)
        {
            if (string.IsNullOrEmpty(promocodeId))
            {
                return BadRequest();
            }

            var redemptions = await _redemptionService.GetByPromocodeId(promocodeId);

            return redemptions != null && redemptions.Any() ? (IActionResult)Ok(redemptions.Select(redemption => _redemptionMapper.Map(redemption)))
                : NotFound();
        }

        [HttpGet("redemptions/{redemptionId}", Name = RouteNames.GetRedemption)]
        public async Task<IActionResult> GetRedemption(string promocodeId, string redemptionId)
        {
            if (string.IsNullOrEmpty(promocodeId) || string.IsNullOrEmpty(redemptionId))
            {
                return BadRequest();
            }
            var redemption = await _redemptionService.Get(promocodeId, redemptionId);

            return redemption != null ? (IActionResult)Ok(_redemptionMapper.Map(redemption))
                : NotFound();
        }

        [HttpPost("redemptions/{redemptionId}/reinstate", Name = RouteNames.ReinstateRedemption)]
        public async Task<IActionResult> Reinstate(string promocodeId, string redemptionId)
        {
            if (string.IsNullOrEmpty(promocodeId) || string.IsNullOrEmpty(redemptionId))
            {
                return BadRequest();
            }

            await _redemptionService.Reinstate(promocodeId, redemptionId);
            _vortexEventPublisher.NotifyVortexOfPromocodeReinstate(promocodeId, redemptionId, _headerService);

            return Ok();
        }

        [HttpGet("redemptions/{redemptionId}/ledger", Name = RouteNames.GetLedger)]
        public async Task<IActionResult> GetLedger(string promocodeId, string redemptionId)
        {
            if (string.IsNullOrEmpty(promocodeId) || string.IsNullOrEmpty(redemptionId))
            {
                return BadRequest();
            }

            var ledger = await _ledgerService.Get(promocodeId, redemptionId);

            var mappedLedger = _ledgerMapper.Map(ledger);

            return Ok(mappedLedger);
        }

        [HttpPost("redemptions/{redemptionId}/ledger/quotes", Name = RouteNames.CreateQuote)]
        public async Task<IActionResult> CreateLedgerQuote(string promocodeId, string redemptionId, [FromBody]QuoteRequest[] quoteRequests)
        {
            if (string.IsNullOrEmpty(promocodeId) || string.IsNullOrEmpty(redemptionId))
            {
                return BadRequest();
            }

            var mappedRequests = quoteRequests.Select(_ledgerMapper.Map);

            var promoQuotes = await _ledgerService.CreateQuotes(promocodeId, redemptionId, mappedRequests);

            var mappedQuotes = promoQuotes.Select(x => _ledgerMapper.Map(promocodeId, redemptionId, x)).ToList();

            return Ok(mappedQuotes);
        }

        [HttpGet("redemptions/{redemptionId}/ledger/quotes/{quoteId}", Name = RouteNames.GetQuote)]
        public async Task<IActionResult> GetLedgerQuote(string promocodeId, string redemptionId, string quoteId)
        {
            if (string.IsNullOrEmpty(promocodeId) || string.IsNullOrEmpty(redemptionId) || string.IsNullOrEmpty(quoteId))
            {
                return BadRequest();
            }

            var quote = await _ledgerService.GetQuote(promocodeId, redemptionId, quoteId);

            return quote != null ? (IActionResult)Ok(_ledgerMapper.Map(promocodeId, redemptionId, quote))
                : NotFound();
        }

        [HttpPost("redemptions/{redemptionId}/ledger/quotes/{quoteId}/forfeit", Name = RouteNames.ForfeitQuote)]
        public async Task<IActionResult> ForfeitLedgerQuote(string promocodeId, string redemptionId, string quoteId)
        {
            if (string.IsNullOrEmpty(promocodeId) || string.IsNullOrEmpty(redemptionId) || string.IsNullOrEmpty(quoteId))
            {
                return BadRequest();
            }

            await _ledgerService.ForfeitQuote(promocodeId, redemptionId, quoteId);

            return Ok();
        }

        [HttpPost("redemptions/{redemptionId}/ledger/link", Name = RouteNames.CreateLink)]
        public async Task<IActionResult> Link(string promocodeId, string redemptionId, [FromBody]LinkProductRequest request)
        {
            if (string.IsNullOrEmpty(promocodeId) || string.IsNullOrEmpty(redemptionId))
            {
                return BadRequest("PromocodeId, RedemptionId are required");
            }

            if (request.OriginalProductAmount != null && request.TargetProducts.Count != 1)
            {
                return BadRequest("Original product amount can be provided on when linking with single target product");
            }

            var targetProductsUris = request.TargetProducts.Select(x => x.ProductUri);

            if (targetProductsUris.Any(x => x.OriginalString.Equals(request.OriginalProductUri.ToString())))
            {
                return BadRequest("ProductUri can't be equal to OriginalProductUri");
            }
    
            var targetProductsCurrencyCode = request.TargetProducts
                .Select(x => x.ProductAmount.CurrencyCode)
                .Distinct()
                .SingleOrDefault();
            if (targetProductsCurrencyCode == null
                || (request.OriginalProductAmount != null && request.OriginalProductAmount.CurrencyCode != targetProductsCurrencyCode))
            {
                return BadRequest("Product Currency Codes not matching");
            }

            var linkRequest = _ledgerMapper.Map(request);

            var link = await _ledgerService.Link(promocodeId, redemptionId, linkRequest);

            return link != null ? (IActionResult)Ok(_ledgerMapper.Map(promocodeId, redemptionId, link))
                : NotFound();
        }

        [HttpPost("redemptions/{redemptionId}/ledger/link/{linkId}/revert", Name = RouteNames.RevertLink)]
        public async Task<IActionResult> RevertLink(string promocodeId, string redemptionId, string linkId)
        {
            if (string.IsNullOrEmpty(promocodeId) || string.IsNullOrEmpty(redemptionId) || string.IsNullOrEmpty(linkId))
            {
                return BadRequest("PromocodeId, RedemptionId and LinkId are required");
            }

            await _ledgerService.RevertLink(promocodeId, redemptionId, linkId);

            return Ok();
        }
    }
}
