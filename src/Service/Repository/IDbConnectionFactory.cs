using System.Data;
using System.Threading.Tasks;

namespace Trainline.PromocodeService.Service.Repository
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateOpenConnectionAsync();
    }
}
