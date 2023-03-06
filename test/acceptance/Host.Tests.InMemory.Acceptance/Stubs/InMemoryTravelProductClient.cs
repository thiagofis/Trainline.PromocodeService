using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trainline.ProductJsonApiDeserialisation.AsyncModel;
using Trainline.PromocodeService.ExternalServices.TravelProduct;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs
{
    public class InMemoryTravelProductClient : ITravelProductClient
    {
        private readonly Dictionary<Uri, AsyncTravelProtocolProduct> _travelProtocolProductStore =
            new Dictionary<Uri, AsyncTravelProtocolProduct>();

        public Task<AsyncTravelProtocolProduct> GetAsyncProductAsync(Uri productUri)
        {
            return Task.FromResult(_travelProtocolProductStore[productUri]);
        }

        public void SetAsyncProduct(Uri productUri, AsyncTravelProtocolProduct travelProtocolProduct)
        {
            _travelProtocolProductStore.Add(productUri, travelProtocolProduct);
        }
    }
}
