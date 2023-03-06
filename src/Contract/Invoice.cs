using System;
using System.Collections.Generic;

namespace Trainline.PromocodeService.Contract
{
    public class Invoice
    {
        public string Id { get; set; }

        public ICollection<Item> ProductItems { get; set; }
        
        public string CurrencyCode { get; set; }
    }
}
