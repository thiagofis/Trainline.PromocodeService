using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trainline.PromocodeService.Service.Repository;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance
{
    public class InMemoryRedemptionRepository : IRedemptionRepository
    {
        private Dictionary<string, ICollection<Redemption>> _entities;

        public InMemoryRedemptionRepository()
        {
            _entities = new Dictionary<string, ICollection<Redemption>>();
        }

        public Task<ICollection<Redemption>> GetByPromocodeId(string promocodeId)
        {
            return Task.FromResult(_entities.ContainsKey(promocodeId) ? _entities[promocodeId] : null);
        }

        public Task<Redemption> Get(string promocodeId, string redemptionId)
        {
            return Task.FromResult(_entities.ContainsKey(promocodeId) ? _entities[promocodeId].FirstOrDefault(x => x.RedemptionId == redemptionId) : null);
        }

        public Task<Redemption> Add(Redemption redemption)
        {
            var promocodeId = redemption.PromocodeId;
            if (!_entities.ContainsKey(promocodeId))
            {
                _entities.Add(promocodeId, new List<Redemption>());
            }

            redemption.Id = _entities[promocodeId].Count + 1;
            _entities[promocodeId].Add(redemption);
            return Task.FromResult(redemption);
        }
    }
}
