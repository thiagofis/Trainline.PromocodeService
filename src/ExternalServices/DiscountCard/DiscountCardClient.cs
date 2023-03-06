using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Trainline.NetStandard.StandardHeaders.Services;
using System.Net.Http.Formatting;
using Trainline.PromocodeService.ExternalServices.DiscountCard;
using Trainline.PromocodeService.ExternalServices.DiscountCard.Contract;
using Trainline.PromocodeService.ExternalServices.Http.Requests;
using Trainline.NetStandard.StandardHeaders.Extensions;

namespace Trainline.PromocodeService.Service
{
    public class DiscountCardClient : IDiscountCardClient
    {
        private readonly IHeaderService _headerService;
        private readonly HttpClient _httpClient;
        public const string AcceptHeader = "application/vnd.trainline.discountcardprotocol.v1+json";

        public DiscountCardClient(HttpClient httpClient, IHeaderService headerService)
        {
            _headerService = headerService;
            _httpClient = httpClient;
        }
        
        public async Task<DiscountCard> GetDiscountCardDetailsAsync(Uri productUri)
        {
            var conversationId = _headerService.GetConversationId();
            var contextUri = _headerService.GetContextUri();

            var request = Request.CreateRequest(HttpMethod.Get, productUri, AcceptHeader, conversationId, contextUri);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new DiscountCardClientException(await response.Content.ReadAsAsync<Error>());
            }

            var discountCard = await response.Content.ReadAsAsync<DiscountCard>(GetMediaTypeFormatters());
            return discountCard;
        }

        private IEnumerable<JsonMediaTypeFormatter> GetMediaTypeFormatters()
        {
            var jsonMediaTypeFormatter = new JsonMediaTypeFormatter();
            jsonMediaTypeFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(AcceptHeader));
            yield return jsonMediaTypeFormatter;
        }

    }
}
