using System;
using System.Threading.Tasks;
using Trainline.ProductJsonApiDeserialisation.AsyncModel;
using Trainline.ProductJsonApiDeserialisation.ProductService;

namespace Trainline.PromocodeService.ExternalServices.TravelProduct
{
    public interface ITravelProductClient
    {
        Task<AsyncTravelProtocolProduct> GetAsyncProductAsync(Uri productUri);
    }
}
