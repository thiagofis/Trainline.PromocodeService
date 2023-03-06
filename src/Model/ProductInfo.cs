using System;

namespace Trainline.PromocodeService.Model
{
    public class ProductInfo
    {
        public long Id { get; set; }

        public Uri ProductUri { get; set; }

        public decimal ProductPrice { get; set; }

        public Uri RootProductUri { get; set; }

        public string LinkId { get; set; }
    }
}
