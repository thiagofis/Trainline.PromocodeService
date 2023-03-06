using System;
using System.Collections.Generic;
using System.Text;

namespace Trainline.PromocodeService.Model
{
    public class Redeemed : IRedemption
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string PromocodeId { get; set; }
        public string CampaignName { get; set; }
        public ICollection<DiscountInvoiceInfo> InvoiceInfos { get; set; }
    }
}
