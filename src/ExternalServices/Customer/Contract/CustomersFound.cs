using System.Linq;

namespace Trainline.PromocodeService.ExternalServices.Customer.Contract
{
    public class CustomersFound
    {
        public Customer[] Customers { get; set; } = new Customer[0];
        public Error[] Errors { get; set; }
        public bool Any => Customers.Any();
    }
}
