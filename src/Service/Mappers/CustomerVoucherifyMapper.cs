using System;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.Customer;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute.Contract;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;

namespace Trainline.PromocodeService.Service.Mappers
{
    public class CustomerVoucherifyMapper : ICustomerVoucherifyMapper
    {
        private readonly ICustomerServiceClient _customerServiceClient;
        private readonly ICustomerAttributeClient _customerAttributeClient;

        public CustomerVoucherifyMapper(ICustomerServiceClient customerServiceClient, ICustomerAttributeClient customerAttributeClient)
        {
            _customerServiceClient = customerServiceClient;
            _customerAttributeClient = customerAttributeClient;
        }

        public async Task<Customer> GetCustomer(Uri? customerUri)
        {
            if (customerUri != null)
            {
                var customer = await _customerServiceClient.GetCustomer(customerUri);
                return await IdentifiedCustomer(customer);
            }
            else
            {
                return UnidentifiedCustomer();
            }
        }

        private async Task<Customer> IdentifiedCustomer(ExternalServices.Customer.Contract.Customer customer)
        {
            var attributes = await _customerAttributeClient.GetCustomerAttributes(customer.Id);

            return new Customer
            {
                metadata = new CustomerMetadata
                {
                    isnew_status = CustomerIsNewStatus(attributes),
                    railcard_status =  CustomerHasBoughtRailcard(attributes),
                    activity_status = CustomerIsActive(attributes)
                },
                source_id = customer.Id
            };
        }

        private Customer UnidentifiedCustomer()
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

        private string CustomerIsNewStatus(CustomerAttributes attributes)
        {
            return attributes.IsNewCustomer() ? "newCustomer" : "repeatCustomer";
        }

        private string CustomerHasBoughtRailcard(CustomerAttributes attributes)
        {
            var hasBought = attributes.GetValue<bool>(Common.CustomerAttributeNames.BoughtRailcard);
            return hasBought ? "hasBoughtRailcard" : "firstTime";
        }

        private string CustomerIsActive(CustomerAttributes attributes)
        {
            var isActive = attributes.GetValue<bool>(Common.CustomerAttributeNames.IsActive);
            return isActive ? "active" : "disabled";
        }
    }
}
