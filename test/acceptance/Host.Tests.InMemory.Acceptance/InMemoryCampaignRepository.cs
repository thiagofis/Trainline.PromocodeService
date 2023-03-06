using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trainline.PromocodeService.Service.Repository;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance
{
    public class InMemoryCampaignRepository : ICampaignRepository
    {
        public List<Campaign> _entities;
        public InMemoryCampaignRepository()
        {
            _entities = new List<Campaign>();
        }

        public Task<Campaign> Get(string campaignId)
        {
            return Task.FromResult(_entities.FirstOrDefault(x => x.CampaignId == campaignId));
        }

        public Task<Campaign> Add(Campaign campaign)
        {
            campaign.Id = _entities.Any() ? _entities.Max(x => x.Id) + 1 : 1;
            _entities.Add(campaign);

            return Task.FromResult(campaign);
        }

        public Task<Campaign> Update(Campaign campaign)
        {
            _entities.Remove(_entities.Single(x => x.CampaignId == campaign.CampaignId));
            _entities.Add(campaign);
            return Task.FromResult(campaign);
        }
    }
}
