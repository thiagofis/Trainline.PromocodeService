using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Trainline.VortexPublisher;

namespace Trainline.PromocodeService.Host.Vortex
{
    [VortexSchema(version: "1.1.1")]
    [Description("Sent every time a promocode is validated from the ECommerce PromocodeService")]
    public class PromocodeValidated : VortexEvent
    {
        [Description("PromocodeId")]
        [Required]
        public string PromocodeId { get; set; }

        [Description("CampaignName")]
        [Required]
        public string CampaignName { get; set; }

        [Description("Discounts generated as part of the validation.")]
        [Required]
        public IEnumerable<DiscountItem> DiscountItems { get; set; }

        [Description("CustomerUri")]
        public string CustomerUri { get; set; }

        public override string GetPartitionKey() => PromocodeId;
    }

    public class DiscountItem
    {
        [Description("Id of an invoice discount is assign to")]
        [Required]
        public string InvoiceId { get; set; }

        [Description("Currency code in with discount is applied")]
        [Required]
        public string CurrencyCode { get; set; }

        [Description("Id of a product that this discount is assign to")]
        [Required]
        public string ProductId { get; set; }

        [Description("Uri of a product that this discount is assign to")]
        [Required]
        public Uri ProductUri { get; set; }

        [Description("Vendor of linked product")]
        [Required]
        public string Vendor { get; set; }

        [Description("Discount amount. Should be negative to reflect it's a discount")]
        [Required]
        public decimal Amount { get; set; }

    }
}
