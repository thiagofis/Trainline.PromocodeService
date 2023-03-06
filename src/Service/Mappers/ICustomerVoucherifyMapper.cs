using System;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;

namespace Trainline.PromocodeService.Service.Mappers
{
    public interface ICustomerVoucherifyMapper
    {
        Task<Customer> GetCustomer(Uri? customerUri);
    }
}
