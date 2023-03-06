using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Trainline.VortexPublisher;

namespace Trainline.PromocodeService.Host.Vortex
{
    [VortexSchema(version: "2.1.0")]
    [Description("Sent every time a new customer promocode is requested.")]
    public class NewCustomerPromocode : VortexEvent
    {
        [Description("The customer email address.")]
        [Required]
        public string Email { get; set; }

        [Description("The customer first name.")]
        [Required]
        public string FirstName { get; set; }

        [Description("The customer last name.")]
        [Required]
        public string LastName { get; set; }

        [Description("The customer unique identifier.")]
        [Required]
        public string CustomerId { get; set; }

        [Description("The campaign identifier on the external platform. eg. Voucherify")]
        [Required]
        public string ExternalCampaignId { get; set; }

        [Description("The customer's locale eg. en-gb")]
        [Required]
        public string Locale { get; set; }

        public override string GetPartitionKey() => Email;
    }
}
