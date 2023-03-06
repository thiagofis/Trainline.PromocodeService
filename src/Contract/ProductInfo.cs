using System;

namespace Trainline.PromocodeService.Contract
{
    public class ProductInfo
    {
        public Uri ProductUri { get; set; }

        public decimal ProductPrice { get; set; }

        public Uri RootProductUri { get; set; }

        public string LinkId { get; set; }
    }
}
