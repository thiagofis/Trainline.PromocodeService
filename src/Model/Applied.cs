using System;
using System.Collections.Generic;
using System.Text;

namespace Trainline.PromocodeService.Model
{
    public class Applied
    {
        public string CampaignName { get; set; }
        public List<DiscountInvoiceInfo> DiscountInvoiceInfo { get; set; }
    }
}
