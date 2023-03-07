using System;

namespace Trainline.PromocodeService.Service.Repository.Entities
{
    public class CampaignEntity
    {
        public int Id { get; set; }
        public string CampaignId { get; set; }
        public bool Redeemable { get; set; } = true;
        public DateTime ExpirationDate { get; set; }
    }
}
