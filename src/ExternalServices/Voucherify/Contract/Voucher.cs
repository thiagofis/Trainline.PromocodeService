using System;
using System.Collections.Generic;

namespace Trainline.PromocodeService.ExternalServices.Voucherify.Contract
{
    public class Voucher
    {
        public string Code { get; set; }

        public string Type { get; set; }

        public string CampaignId { get; set; }

        public string Campaign { get; set; }

        public bool Active { get; set; }

        public Discount Discount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime ExpirationDate { get; set; }
        
        public VoucherRedemptions Redemption { get; set; }
        
        public Dictionary<string, string> Metadata { get; set; }
        
        public ValidationRulesAssignments ValidationRulesAssignments { get; set; }
    }
}
