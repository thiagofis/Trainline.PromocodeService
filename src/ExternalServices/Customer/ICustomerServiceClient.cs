using System;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.Customer.Contract;
using Trainline.PromocodeService.ExternalServices.Http.Requests;

namespace Trainline.PromocodeService.ExternalServices.Customer
{
    public interface ICustomerServiceClient
    {
        Task<Contract.Customer> GetCustomer(Uri customerUri);
        Task<CustomersFound> GetCustomerByEmail(SearchCriteria searchCriteria);
        Task<CustomerRegistered> RegisterCustomerImplicitly(ImplicitRegistration implicitRegistration);
    }
}
