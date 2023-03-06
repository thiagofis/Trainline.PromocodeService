using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute.Contract;
using Trainline.PromocodeService.ExternalServices.Exceptions;
using Trainline.PromocodeService.ExternalServices.Http.Requests;

namespace Trainline.PromocodeService.ExternalServices.CustomerAttribute
{
    public class CustomerAttributeClient : ICustomerAttributeClient
    {
        private readonly IHttpRequestClient _httpRequestClient;
        private readonly CustomerAttributeSettings _settings;
        private readonly ILogger<CustomerAttributeClient> _logger;

        public CustomerAttributeClient(IHttpRequestClient httpRequestClient, IOptions<CustomerAttributeSettings> settings,
            ILogger<CustomerAttributeClient> logger)
        {
            _httpRequestClient = httpRequestClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<CustomerAttributes> GetCustomerAttributes(string customerId)
        {
            if (!_settings.IsServiceEnabled)
            {
                throw new CustomerAttributeServiceIsNotAvailableException($"Service is not available. BaseUri is null or empty.");
            }

            var uri = new Uri($"{_settings.BaseUri}/customers/{customerId}");
            _logger.LogInformation("Get CustomerAttribute. Uri={Uri} CustomerId={CustomerId}", uri, customerId);

            try
            {
                var httpResult = await _httpRequestClient.GetAsync<CustomerAttributes>(uri);
                return httpResult.Result;
            }
            catch(HttpResponseException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return CustomerAttributes.New(customerId);
                }
                throw;
            }
        }
    }
}
