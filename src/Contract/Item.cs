using System;

namespace Trainline.PromocodeService.Contract
{
    public class Item
    {
        public string ProductId { get; set; }

        public Uri ProductUri { get; set; }

        public string Vendor { get; set; }

        public decimal Amount { get; set; }
    }
}
