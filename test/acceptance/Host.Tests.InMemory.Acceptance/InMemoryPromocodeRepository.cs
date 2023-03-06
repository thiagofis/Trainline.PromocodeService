using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trainline.PromocodeService.Service.Repository;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance
{
    public class InMemoryPromocodeRepository : IPromocodeRepository
    {
        private List<Promocode> _entities;

        public InMemoryPromocodeRepository()
        {
            _entities = new List<Promocode>();
        }

        public Task<Promocode> Add(Promocode promocode)
        {
            promocode.Id = _entities.Any() ? _entities.Max(x =>x.Id) + 1 : 1;
            _entities.Add(promocode);

            return Task.FromResult(promocode);
        }

        public Task<Promocode> Update( Promocode promocode)
        {
            _entities.Remove(_entities.Single(x => x.Code == promocode.Code));
            _entities.Add(promocode);
            return Task.FromResult(promocode);
        }

        public Task<Promocode> GetByPromocodeId(string promocodeId)
        {
            return Task.FromResult(_entities.FirstOrDefault(x => x.PromocodeId == promocodeId));
        }

        public Task<Promocode> GetByCode(string code)
        {
            return Task.FromResult(_entities.FirstOrDefault(x => x.Code == code));
        }
    }
}
