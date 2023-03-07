using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Trainline.PromocodeService.ExternalServices.Customer.Contract;
using Trainline.PromocodeService.ExternalServices.Customer.Exceptions;
using Trainline.PromocodeService.ExternalServices.Http.Requests;

namespace Trainline.PromocodeService.ExternalServices.Customer
{
    public class AlfaCustomerServiceClient : ICustomerServiceClient
    {
        private readonly IHttpRequestClient _httpRequestClient;
        private readonly CustomerServiceSettings _settings;
        private readonly ILogger<CustomerServiceClient> _logger;

        public AlfaCustomerServiceClient(IHttpRequestClient httpRequestClient, IOptions<CustomerServiceSettings> settings, ILogger<CustomerServiceClient> logger)
        {
            _httpRequestClient = httpRequestClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<CustomersFound> GetCustomerByEmail(SearchCriteria searchCriteria)
        {
            var uri = new Uri($"{_settings.AlfaBaseUri}/Customers/Search");
            _logger.LogInformation("Fetching customer by email. Uri={Uri} Payload={Payload}", uri, searchCriteria);
            var httpResult = await _httpRequestClient.PostAsync<SearchCriteria, CustomersFound>(uri, searchCriteria);
            return httpResult.Result;
        }

        public Task<CustomerRegistered> RegisterCustomerImplicitly(ImplicitRegistration implicitRegistration)
        {
            throw new NotSupportedException("Alpha provider does not support this operation.");
        }

        public async Task<Contract.Customer> GetCustomer(Uri customerUri)
        {
            throw new NotSupportedException("Alpha provider does not support this operation.");
        }
    }
}
