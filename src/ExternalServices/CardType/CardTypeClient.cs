using JsonApiSerializer;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Trainline.NetStandard.StandardHeaders.Extensions;
using Trainline.NetStandard.StandardHeaders.Services;
using Trainline.PromocodeService.ExternalServices.CardType.Contract;
using Trainline.PromocodeService.ExternalServices.Http.Requests;

namespace Trainline.PromocodeService.ExternalServices.CardType
{
    public class CardTypeClient : ICardTypeClient
    {
        private readonly IHeaderService _headerService;
        private readonly HttpClient _httpClient;
        public const string AcceptHeader = "application/vnd.trainline.travelprotocol.v3+json";

        public CardTypeClient(HttpClient httpClient, IHeaderService headerService)
        {
            _headerService = headerService;
            _httpClient = httpClient;
        }

        public async Task<string> GetCardTypeCodeAsync(Uri cardTypeUri)
        {
            var conversationId = _headerService.GetConversationId();
            var contextUri = _headerService.GetContextUri();

            var request = Request.CreateRequest(HttpMethod.Get, cardTypeUri, AcceptHeader, conversationId, contextUri);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new CardTypeClientException(await response.Content.ReadAsAsync<Error>());
            }
            var cardType = JsonConvert.DeserializeObject<Contract.CardType>(await response.Content.ReadAsStringAsync(), new JsonApiSerializerSettings());

            return cardType.Code;
        }

    }
}
