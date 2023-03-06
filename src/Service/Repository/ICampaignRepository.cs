using System.Threading.Tasks;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Service.Repository
{
    public interface  ICampaignRepository
    {
        Task<CampaignEntity> Get(string campaignId);

        Task<CampaignEntity> Add(CampaignEntity campaignEntity);

        Task<CampaignEntity> Update(CampaignEntity campaignEntity);
    }
}
