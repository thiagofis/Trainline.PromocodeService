using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Trainline.VortexPublisher;

namespace Trainline.PromocodeService.Host.Vortex
{
    [VortexSchema(version: "1.0.2")]
    [Description("Sent every time a promocode is created from the ECommerce PromocodeService")]
    public class PromocodeCreated : VortexEvent
    {
        [Description("PromocodeId")]
        [Required]
        public string PromocodeId { get; set; }

        [Description("CampaignName")]
        [Required]
        public string CampaignName { get; set; }

        [Description("Code")]
        [Required]
        public string Code { get; set; }

        [Description("Promocode start date")]
        [Required]
        public DateTimeOffset ValidityStartDate { get; set; }

        [Description("Promocode end date")]
        [Required]
        public DateTimeOffset ValidityEndDate { get; set; }

        public Discount Discount { get; set; }

        [Description("Number of times promocode can be Redeemed")]
        [Required]
        public int? RedemptionQuantity { get; set; }

        [Description("Number of times promocode has been Redeemed")]
        [Required]
        public int RedeemedQuantity { get; set; }

        [Description("Order minimum amount to be used")]
        [Required]
        public string OrderMinimumAmount { get; set; }
        
        [Description("Currency in with promocode can be use")]
        [Required]
        public string CurrencyCode { get; set; }

        public override string GetPartitionKey() => PromocodeId;
    }

    public class Discount
    {
        [Description("Type of a discount")]
        [Required]
        public string Type { get; set; }
        
        [Description("Amount of discount in given type")]
        public decimal Amount { get; set; }
    }

}
