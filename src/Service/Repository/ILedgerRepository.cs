using System.Threading.Tasks;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Service.Repository
{
    public interface ILedgerRepository
    {
        Task<Ledger> Get(string promocodeId, string redemptionId);

        Task<Ledger> Add(Ledger ledger);

        Task<Ledger> Update(Ledger ledger);

        Task RemoveLink(string promocodeId, string redemptionId, string linkId);
    }
}
