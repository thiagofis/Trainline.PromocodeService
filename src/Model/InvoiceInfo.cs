using System;
using System.Collections.Generic;

namespace Trainline.PromocodeService.Model
{
    public class InvoiceInfo
    {
        public string Id { get; set; }

        public ICollection<ProductItem> ProductItems { get; set; }
        
        public string CurrencyCode { get; set; }
    }
}
