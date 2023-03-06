using System;
using System.Collections.Generic;
using System.Text;

namespace Trainline.PromocodeService.Service.Repository.Entities
{
    public class Promocode
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public DateTime ValidityStartDate { get; set; }

        public DateTime ValidityEndDate { get; set; }

        public DateTime RetentionDate { get; set; }

        public int? RedemptionQuantity { get; set; }

        public int RedeemedQuantity { get; set; }

        public string DiscountType { get; set; }

        public decimal DiscountAmount { get; set; }

        public string CurrencyCode { get; set; }

        public string ValidationRuleId { get; set; }

        public string PromocodeId { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }

        public string ProductType { get; set; }

    }
}
