using System;
using System.Collections.Generic;
using System.Text;

namespace Trainline.PromocodeService.Contract
{
    public class Redeemed
    {
        public string RedemptionId { get; set; }

        public string Code { get; set; }

        public ICollection<DiscountItem> DiscountItems { get; set; }

        public Dictionary<string, Link> Links { get; set; }
    }
}
