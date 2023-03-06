using System.Collections.Generic;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute.Contract;
using Trainline.PromocodeService.ExternalServices.Http.Requests;

namespace Trainline.PromocodeService.ExternalServices.CustomerAttribute
{
    public interface ICustomerAttributeClient
    {
        Task<CustomerAttributes> GetCustomerAttributes(string customerId);
    }
}
