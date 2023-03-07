using System;
using System.ComponentModel.DataAnnotations;

namespace Trainline.PromocodeService.Contract
{
    public class NewCampaign
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime StartTime  { get; set; }

        [Required]
        public DateTime EndTime  { get; set; }

        [Required]
        public string ExternalId { get; set; }

        [Required]
        public string Locale { get; set; }
    }

    public class UpdateCampaign : NewCampaign
    {
        [Required]
        public Guid Id { get; set; }
    }
}
