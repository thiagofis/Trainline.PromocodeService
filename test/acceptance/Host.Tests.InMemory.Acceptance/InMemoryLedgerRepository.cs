using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trainline.PromocodeService.Service.Repository;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance
{

    public class InMemoryLedgerRepository : ILedgerRepository
    {
        private readonly Dictionary<(string, string), Ledger> _ledgers;
        private long _nextId;

        public InMemoryLedgerRepository()
        {
            _ledgers = new Dictionary<(string, string), Ledger>();
            _nextId = 1;
        }
        public Task<Ledger> Get(string promocodeId, string redemptionId)
        {
            return Task.FromResult(_ledgers[(promocodeId, redemptionId)]);
        }

        public Task<Ledger> Add(Ledger ledger)
        {
            ledger.Id = _nextId++;
            _ledgers.Add((ledger.PromocodeId, ledger.RedemptionId), ledger);

            return Task.FromResult(ledger);
        }

        public Task<Ledger> Update(Ledger ledger)
        {
            _ledgers[(ledger.PromocodeId, ledger.RedemptionId)] = ledger;

            return Task.FromResult(ledger);
        }

        public Task RemoveLink(string promocodeId, string redemptionId, string linkId)
        {
            var ledger = _ledgers[(promocodeId, redemptionId)];

            ledger.Products = ledger.Products.Where(x => x.LinkId != linkId).ToList();
            ledger.Lines = ledger.Lines.Where(x => x.LinkId != linkId).ToList();

            return Task.FromResult(ledger);
        }
    }
}
