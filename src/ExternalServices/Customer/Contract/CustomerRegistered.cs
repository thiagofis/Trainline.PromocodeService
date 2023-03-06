using System.Linq;

namespace Trainline.PromocodeService.ExternalServices.Customer.Contract
{
    public class CustomerRegistered
    {
        public string CustomerId { get; set; }
        public Error[] Errors { get; set; } = new Error[0];
        public bool HasErrors => Errors.Any();

        public override string ToString()
        {
            return $"{nameof(Errors)}: [{string.Join(',',Errors.Select(e => e.ToString()))}]";
        }

    }
}
