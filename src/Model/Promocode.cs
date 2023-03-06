using System;
using System.Collections.Generic;

namespace Trainline.PromocodeService.Model
{
    public class Promocode
    {
        public string PromocodeId { get; set; }
        public int Id { get; set; }

        public string Code { get; set; }

        public DateTime ValidityStartDate { get; set; }

        public DateTime ValidityEndDate { get; set; }

        public int? RedemptionQuantity { get; set; }

        public int RedeemedQuantity { get; set; }

        public Discount Discount { get; set; }

        public string CurrencyCode { get; set; }

        public string ProductType { get; set; }

        public string CampaignName { get; set; }

        public IEnumerable<ValidationRule> ValidationRules { get; set; }

        public PromocodeState State { get; set; }

        public PromocodeType Type { get; set; } = PromocodeType.Discount;
    }
}
