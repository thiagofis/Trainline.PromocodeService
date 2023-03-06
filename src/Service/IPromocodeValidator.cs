using System.Threading.Tasks;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Service
{
    public interface IPromocodeValidator
    {
        Task Validate(string code);
    }
}
