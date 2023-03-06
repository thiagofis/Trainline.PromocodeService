using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trainline.NetStandard.StandardHeaders.Extensions;
using Trainline.NetStandard.StandardHeaders.Services;
using Trainline.PromocodeService.Common.Formatting;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Host.Constants;
using Trainline.PromocodeService.Host.Extensions;
using Trainline.PromocodeService.Service;
using Trainline.VortexPublisher.EventPublishing;

namespace Trainline.PromocodeService.Host.Controllers
{
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignValidator<NewCustomerCampaignApplication, NewCustomerCampaignEligibilityData> _campaignValidator;
        private readonly IVortexEventPublisher _publisher;
        private readonly IHeaderService _headerService;

        public CampaignController(ICampaignValidator<NewCustomerCampaignApplication, NewCustomerCampaignEligibilityData> campaignValidator, IVortexEventPublisher publisher, IHeaderService headerService)
        {
            _campaignValidator = campaignValidator;
            _headerService = headerService;
            _publisher = publisher;
        }

        [HttpPost("campaign/newCustomer", Name = RouteNames.NewCustomerCampaign)]
        public async Task<IActionResult> NewCustomer([FromBody] ApplyNewCustomerCampaign request)
        {
            var campaignApplication = new NewCustomerCampaignApplication(
                request.Email,
                request.FirstName,
                request.LastName,
                request.ExternalCampaignId,
                request.Locale,
                _headerService.GetContextUri(),
                _headerService.GetConversationId());

           var eligibilityData = await _campaignValidator.ValidateEligibility(campaignApplication);

           _publisher.NotifyVortexOfNewCustomerPromocode(eligibilityData, _headerService);

            return Accepted();
        }
    }
}
