using System.Collections.Generic;
using System.Threading.Tasks;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Service.Repository
{
    public interface IRedemptionRepository
    {
        Task<ICollection<Redemption>> GetByPromocodeId(string promocodeId);

        Task<Redemption> Get(string promocodeId, string redemptionId);

        Task<Redemption> Add(Redemption redemption);
    }
}
