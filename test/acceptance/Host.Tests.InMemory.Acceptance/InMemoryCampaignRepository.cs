using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trainline.PromocodeService.Service.Repository;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance
{
    public class InMemoryCampaignRepository : ICampaignRepository
    {
        public List<CampaignEntity> _entities;
        public InMemoryCampaignRepository()
        {
            _entities = new List<CampaignEntity>();
        }

        public Task<CampaignEntity> Get(string campaignId)
        {
            return Task.FromResult(_entities.FirstOrDefault(x => x.CampaignId == campaignId));
        }

        public Task<CampaignEntity> Add(CampaignEntity campaignEntity)
        {
            campaignEntity.Id = _entities.Any() ? _entities.Max(x => x.Id) + 1 : 1;
            _entities.Add(campaignEntity);

            return Task.FromResult(campaignEntity);
        }

        public Task<CampaignEntity> Update(CampaignEntity campaignEntity)
        {
            _entities.Remove(_entities.Single(x => x.CampaignId == campaignEntity.CampaignId));
            _entities.Add(campaignEntity);
            return Task.FromResult(campaignEntity);
        }
    }
}
