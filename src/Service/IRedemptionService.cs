using System.Collections.Generic;
using System.Threading.Tasks;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service
{
    public interface IRedemptionService
    {
        Task<ICollection<Redemption>> GetByPromocodeId(string promocodeId);

        Task<Redemption> Get(string code, string redemptionId);

        Task Reinstate(string code, string redemptionId);
    }
}
