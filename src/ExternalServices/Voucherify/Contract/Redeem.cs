namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public class Redeem
    {
        public Customer Customer { get; set; }

        public Order Order { get; set; }
    }
}
