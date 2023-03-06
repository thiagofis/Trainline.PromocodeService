using System;
using System.Collections.Generic;

namespace Trainline.PromocodeService.Contract
{
    public class Promocode
    {
        public string PromocodeId { get; set; }
        public string Code { get; set; }

        public string ValidityStartDate { get; set; }

        public string ValidityEndDate { get; set; }

        public Discount Discount { get; set; }

        public string CurrencyCode { get; set; }

        public string ProductType { get; set; }

        public string CampaignName { get; set; }

        public Dictionary<string, Link> Links { get; set; }

        public IEnumerable<ValidationRule> ValidationRules { get; set; }

        public PromocodeRedemption Redemption { get; set; }

        public string State { get; set; }

        public string Type { get; set; }
    }

    public class PromocodeRedemption
    {
        public int? RedemptionQuantity { get; set; }
        public int RedeemedQuantity { get; set; }
    }

    public class ValidationRule
    {
        public ValidationRule(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }
}
