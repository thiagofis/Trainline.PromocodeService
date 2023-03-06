using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute.Contract;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs
{
    public class InMemoryCustomerAttributeClient : ICustomerAttributeClient
    {
        private Dictionary<string, CustomerAttributeDetails[]> _CustomerAttributeDetails = new Dictionary<string, CustomerAttributeDetails[]>();

        public void SetCustomerAttributes(string customerId, CustomerAttributeDetails[] customerAttributeDetails)
        {
            _CustomerAttributeDetails[customerId] = customerAttributeDetails;
        }

        public Task<CustomerAttributes> GetCustomerAttributes(string customerId)
        {
            _CustomerAttributeDetails.TryGetValue(customerId, out var value);
            return Task.FromResult(new CustomerAttributes
            {
                Attributes = value
            });
        }

    }
}
