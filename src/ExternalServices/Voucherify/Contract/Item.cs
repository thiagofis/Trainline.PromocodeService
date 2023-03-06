namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public class Item
    {
        public string ProductId { get; set; }

        public string Quantity { get; set; }

        public int Price { get; set; }

        public Product Product { get; set; }
    }
}
