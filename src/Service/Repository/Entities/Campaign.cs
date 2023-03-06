namespace Trainline.PromocodeService.Service.Repository.Entities
{
    public class Campaign
    {
        public int Id { get; set; }
        public string CampaignId { get; set; }
        public bool Redeemable { get; set; } = true;
    }
}
