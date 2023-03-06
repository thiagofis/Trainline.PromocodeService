using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Trainline.PromocodeService.ExternalServices.Customer.Contract;
using Trainline.PromocodeService.ExternalServices.Customer.Exceptions;
using Trainline.PromocodeService.ExternalServices.Http.Requests;

namespace Trainline.PromocodeService.ExternalServices.Customer
{
    public class CustomerServiceClient : ICustomerServiceClient
    {
        private readonly IHttpRequestClient _httpRequestClient;
        private readonly CustomerServiceSettings _settings;
        private readonly ILogger<CustomerServiceClient> _logger;

        public CustomerServiceClient(IHttpRequestClient httpRequestClient, IOptions<CustomerServiceSettings> settings, ILogger<CustomerServiceClient> logger)
        {
            _httpRequestClient = httpRequestClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<CustomersFound> GetCustomerByEmail(SearchCriteria searchCriteria)
        {
            var uri = new Uri($"{_settings.BaseUri}/CustomerService.svc/json/Search");
            _logger.LogInformation("Fetching customer by email. Uri={Uri} Payload={Payload}", uri, searchCriteria);
            var httpResult = await _httpRequestClient.PostAsync<SearchCriteria, CustomersFound>(uri, searchCriteria);
            return httpResult.Result;
        }

        public async Task<CustomerRegistered> RegisterCustomerImplicitly(ImplicitRegistration implicitRegistration)
        {
            var uri = new Uri($"{_settings.BaseUri}/CustomerService.svc/json/registerimplicit");
            _logger.LogInformation("Registering customer implicitly. Uri={Uri} Payload={Payload}", uri, implicitRegistration);
            var httpResult = await _httpRequestClient.PostAsync<ImplicitRegistration, CustomerRegistered>(uri, implicitRegistration);
            var result = httpResult.Result;

            if (result.HasErrors)
            {
                var resultDetails = result.ToString();
                _logger.LogInformation("Registering customer implicitly. Uri={Uri} Details={Details}", uri, resultDetails);
                throw new CustomerImplicitRegistrationException(resultDetails);
            }

            return result;
        }

        public async Task<Contract.Customer> GetCustomer(Uri customerUri)
        {
            _logger.LogInformation("Fetching customer by Uri. Uri={Uri}", customerUri);
            var httpResult = await _httpRequestClient.GetAsync<Contract.Customer>(customerUri);
            return httpResult.Result;
        }
    }
}
