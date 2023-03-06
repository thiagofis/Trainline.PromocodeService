using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Trainline.NetStandard.Headers.Enums;
using Trainline.NetStandard.StandardHeaders.Extensions;
using Trainline.NetStandard.StandardHeaders.Services;
using Trainline.Product.SupportedProtocols;
using Trainline.PromocodeService.ExternalServices.DiscountCard;

namespace Trainline.PromocodeService.Service
{
    public class ProductTypeClient : IProductTypeClient
    {
        private const string DiscountCardProtocolV1MediaType = "application/vnd.trainline.discountcardprotocol.v1+json";
        private const string TravelProtocolV3MediaType = "application/vnd.trainline.travelprotocol.v3+json";

        private const string Travel = "travel";
        private const string Railcard = "railcard";

        private readonly ISupportedProtocolsService _supportedProtocolsService;
        private readonly IHeaderService _headerService;

        public ProductTypeClient(ISupportedProtocolsService supportedProtocolsService, IHeaderService headerService)
        {
            _supportedProtocolsService = supportedProtocolsService;
            _headerService = headerService;
        }

        public async Task<string> GetProductType(Uri productUri)
        {
            var conversationId = _headerService.GetConversationId();
            var contextUri = _headerService.GetContextUri();
            var supportedProtocols = await
                _supportedProtocolsService.SupportedProtocolsAsync(productUri, conversationId, new Uri(contextUri));

            if (supportedProtocols.Contains(DiscountCardProtocolV1MediaType))
            {
                return Railcard;
            }

            if (supportedProtocols.Contains(TravelProtocolV3MediaType))
            {
                return Travel;
            }

            return null;
        }

    }
}
