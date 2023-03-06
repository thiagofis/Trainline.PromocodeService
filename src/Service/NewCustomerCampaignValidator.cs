using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trainline.PromocodeService.Common.Formatting;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.ExternalServices.Context;
using Trainline.PromocodeService.ExternalServices.Context.Contract;
using Trainline.PromocodeService.ExternalServices.Customer;
using Trainline.PromocodeService.ExternalServices.Customer.Contract;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute;
using Trainline.PromocodeService.Service.Exceptions;

namespace Trainline.PromocodeService.Service
{
    public class NewCustomerCampaignValidator : ICampaignValidator<NewCustomerCampaignApplication, NewCustomerCampaignEligibilityData>
    {
        private readonly IContextClient _contextClient;
        private readonly ICustomerServiceClient _customerClient;
        private readonly ICustomerAttributeClient _customerAttributeClient;
        private readonly ILogger<NewCustomerCampaignValidator> _logger;

        public NewCustomerCampaignValidator(IContextClient contextClient, ICustomerServiceClient customerClient, ICustomerAttributeClient customerAttributeClient, ILogger<NewCustomerCampaignValidator> logger)
        {
            _contextClient = contextClient;
            _customerClient = customerClient;
            _customerAttributeClient = customerAttributeClient;
            _logger = logger;
        }

        public async Task<NewCustomerCampaignEligibilityData> ValidateEligibility(NewCustomerCampaignApplication campaignApplication)
        {
            var contextParameters = await _contextClient.GetAsync(campaignApplication.ContextUri);
            var customerId = await GetCustomerIdIfExist(campaignApplication.Email, contextParameters);

            if (!string.IsNullOrEmpty(customerId))
            {
                await ValidateCustomerEligibleForTheCampaign(customerId);
            }
            else
            {
                customerId = await RegisterCustomerImplicitly(campaignApplication.Email, contextParameters);
            }

            return new NewCustomerCampaignEligibilityData(
                customerId,
                campaignApplication.Email,
                campaignApplication.FirstName,
                campaignApplication.LastName,
                campaignApplication.ExternalCampaignId,
                campaignApplication.Locale
            );
        }

        private async Task<string> GetCustomerIdIfExist(string email, ContextParameters contextParameters)
        {
            string customerId = null;
            var searchCriteria = new SearchCriteria(email, contextParameters.ManagedGroupId);
            var customersFound = await _customerClient.GetCustomerByEmail(searchCriteria);

            if (customersFound.Any)
            {
                customerId = customersFound.Customers.Single().Id;
            }

            return customerId;
        }

        private async Task<string> RegisterCustomerImplicitly(string email, ContextParameters contextParameters)
        {
            _logger.LogInformation("No customers were found for the email address. The implicit registration will be performed. Email={Email}, EntryPointId={EntryPointId}, ManagedGroupId={ManagedGroupId}", email.MaskEmail(), contextParameters.EntryPointId, contextParameters.ManagedGroupId);
            var entryParameter = new EntryParameter(contextParameters.EntryPointId, contextParameters.ManagedGroupId);
            var implicitRegistration = new ImplicitRegistration(email, entryParameter);
            var customerRegistered = await _customerClient.RegisterCustomerImplicitly(implicitRegistration);
            return customerRegistered.CustomerId;
        }

        private async Task ValidateCustomerEligibleForTheCampaign(string customerId)
        {
            var customerAttribute = await _customerAttributeClient.GetCustomerAttributes(customerId);
            if (!customerAttribute.IsNewCustomer())
            {
                _logger.LogInformation("The customer is not eligible for the campaign. CustomerId={CustomerId}", customerId);
                throw new CustomerIsNotEligibleForTheCampaignException($"The customer is not eligible for the campaign. CustomerId={customerId}");
            }
        }
    }
}
