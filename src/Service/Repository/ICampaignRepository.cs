using System.Threading.Tasks;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Service.Repository
{
    public interface  ICampaignRepository
    {
        Task<Campaign> Get(string campaignId);

        Task<Campaign> Add(Campaign campaign);

        Task<Campaign> Update(Campaign campaign);
    }
}
