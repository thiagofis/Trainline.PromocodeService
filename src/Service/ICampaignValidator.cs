using System.Threading.Tasks;

namespace Trainline.PromocodeService.Service
{
    public interface ICampaignValidator<TCampaignApplication, TEligibilityData>
    {
        Task<TEligibilityData> ValidateEligibility(TCampaignApplication campaignApplication);
    }
}
