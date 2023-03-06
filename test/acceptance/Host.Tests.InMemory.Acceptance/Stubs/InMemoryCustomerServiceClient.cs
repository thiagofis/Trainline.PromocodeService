using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.Customer;
using Trainline.PromocodeService.ExternalServices.Customer.Contract;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs
{
    public class InMemoryCustomerServiceClient : ICustomerServiceClient
    {
        private Dictionary<Uri, ExternalServices.Customer.Contract.Customer> customers = new Dictionary<Uri, ExternalServices.Customer.Contract.Customer>();

        public Task<ExternalServices.Customer.Contract.Customer> GetCustomer(Uri customerUri)
        {
            return Task.FromResult(customers[customerUri]);
        }

        public Task<CustomersFound> GetCustomerByEmail(SearchCriteria searchCriteria)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerRegistered> RegisterCustomerImplicitly(ImplicitRegistration implicitRegistration)
        {
            throw new NotImplementedException();
        }

        public void AddCustomer(Uri uri, ExternalServices.Customer.Contract.Customer customer)
        {
            customers[uri] = customer;
        }
    }
}
