namespace Trainline.PromocodeService.Service.Repository.Entities
{
    public class ProductInfo
    {
        public long Id { get; set; }

        public long LedgerId { get; set; }

        public string ProductUri { get; set; }

        public decimal ProductPrice { get; set; }

        public string RootProductUri { get; set; }

        public string LinkId { get; set; }
    }
}
