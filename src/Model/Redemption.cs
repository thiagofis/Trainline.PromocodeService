namespace Trainline.PromocodeService.Model
{
    public class Redemption : IRedemption
    {
        public string Id { get; set; }
        public string PromocodeId { get; set; }
        public string CampaignName { get; set; }
    }
}
