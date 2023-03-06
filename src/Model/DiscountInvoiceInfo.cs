using System.Collections.Generic;

namespace Trainline.PromocodeService.Model
{
    public class DiscountInvoiceInfo
    {
        public string Id { get; set; }

        public ICollection<DiscountItem> Items { get; set; }

        public string CurrencyCode { get; set; }
    }
}
