using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.Context.Contract;
using Trainline.PromocodeService.ExternalServices.Http.Requests;

namespace Trainline.PromocodeService.ExternalServices.Context
{
    public interface IContextClient
    {
        Task<ContextParameters> GetAsync(string contextUri);
    }
}
