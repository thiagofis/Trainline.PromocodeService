using System;
using System.Threading.Tasks;

namespace Trainline.PromocodeService.Service
{
    public interface IProductTypeClient
    {
        Task<string> GetProductType(Uri productUri);
    }
}
