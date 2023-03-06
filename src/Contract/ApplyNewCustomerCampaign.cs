using System.ComponentModel.DataAnnotations;

namespace Trainline.PromocodeService.Contract
{
    public class ApplyNewCustomerCampaign
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string ExternalCampaignId { get; set; }

        [Required]
        public string Locale { get; set; }
    }
}
