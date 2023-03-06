using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Trainline.NetStandard.StandardHeaders.Extensions;
using Trainline.NetStandard.StandardHeaders.Services;
using Trainline.ProductJsonApiDeserialisation;
using Trainline.ProductJsonApiDeserialisation.AsyncModel;
using Trainline.ProductJsonApiDeserialisation.Exceptions;
using Trainline.PromocodeService.ExternalServices.Http.Requests;
using UnsupportedMediaTypeException = Trainline.ProductJsonApiDeserialisation.UnsupportedMediaTypeException;

namespace Trainline.PromocodeService.ExternalServices.TravelProduct
{
    public class TravelProductClient : ITravelProductClient
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly IHeaderService _headerService;

        public TravelProductClient(IHttpClientFactory httpClientFactory, IHeaderService headerService)
        {
            _httpClientFactory = httpClientFactory;
            _headerService = headerService;

        }

        public async Task<TProduct> GetProductAsync<TProduct>(Uri productUri, string contextUri, string conversationId)
        {
            var productJsonApiClient = new ProductJsonApiClient(_httpClientFactory.CreateClient(nameof(TravelProductClient)), new Uri(contextUri), conversationId, UserAgentAccessor.GetUserAgent());
            try
            {
                return await productJsonApiClient.RetrieveProductAsync<TProduct>(productUri);
            }
            catch (Exception ex)
            {
                if (ex is UnsupportedMediaTypeException)
                    return default;
                if (ex is HttpRequestException)
                {
                    var statusCode = ((ProductRequestException)ex.InnerException)?.StatusCode;
                    if (HttpStatusCode.NotFound.Equals(statusCode))
                        return default;
                }
                throw;
            }
        }

        public Task<AsyncTravelProtocolProduct> GetAsyncProductAsync(Uri productUri)
        {
            var conversationId = _headerService.GetConversationId();
            var contextUri = _headerService.GetContextUri();

            return GetProductAsync<AsyncTravelProtocolProduct>(productUri, contextUri, conversationId);
        }

       

    }
}
