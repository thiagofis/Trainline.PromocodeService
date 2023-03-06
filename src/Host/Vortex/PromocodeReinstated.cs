using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Trainline.VortexPublisher;

namespace Trainline.PromocodeService.Host.Vortex
{
    [VortexSchema(version: "1.1.0")]
    [Description("Sent every time a promocode is reinstated from the ECommerce PromocodeService")]
    public class PromocodeReinstated : VortexEvent
    {
        [Description("PromocodeId")]
        [Required]
        public string PromocodeId { get; set; }
        
        [Description("Id of a redeption that was reinstated")]
        [Required]
        public string RedemptionId { get; set; }

        [Description("CustomerUri")]
        public string CustomerUri { get; set; }

        public override string GetPartitionKey() => PromocodeId;
    }
}
