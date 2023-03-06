namespace Trainline.PromocodeService.Model
{
    public interface IRedemption
    {
        string Id { get; }
        string PromocodeId { get; }
        string CampaignName { get; }
    }
}
