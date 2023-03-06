using System;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;

namespace Trainline.PromocodeService.Service.Mappers
{
    public class DefaultCustomerVoucherifyMapper : ICustomerVoucherifyMapper
    {
        public async Task<Customer> GetCustomer(Uri? customerUri)
        {
            return new Customer
            {
                metadata = new CustomerMetadata
                {
                    isnew_status = "customerNotIdentified"
                },
                source_id = Guid.NewGuid().ToString()
            };
        }
    }
}
