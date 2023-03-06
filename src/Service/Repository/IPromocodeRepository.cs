using System.Threading.Tasks;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Service.Repository
{
    public interface IPromocodeRepository
    {
        Task<Promocode> GetByPromocodeId(string promocodeId);

        Task<Promocode> GetByCode(string code);

        Task<Promocode> Add(Promocode promocode);

        Task<Promocode> Update(Promocode promocode);
    }
}
