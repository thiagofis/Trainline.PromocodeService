using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Trainline.VortexPublisher;

namespace Trainline.PromocodeService.Host.Vortex
{
    [VortexSchema(version: "1.1.1")]
    [Description("Sent every time a promocode is validated from the ECommerce PromocodeService")]
    public class PromocodeRedeemed : VortexEvent
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
}
