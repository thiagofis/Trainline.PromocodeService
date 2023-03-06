namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public class Discount
    {
        public string Type { get; set; }

        public double PercentOff { get; set; }

        public int AmountOff { get; set; }
    }
}
