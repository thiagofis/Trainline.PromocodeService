using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trainline.Product.SupportedProtocols;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs
{
    public class InMemorySupportedProtocolsService : ISupportedProtocolsService
    {
        public const string DiscountCardProtocolV1MediaType = "application/vnd.trainline.discountcardprotocol.v1+json";
        public const string TravelProtocolV3MediaType = "application/vnd.trainline.travelprotocol.v3+json";
        public const string UnsupportedProtocolType = "application/vnd.trainline.unsopportedprotocol.v1+json";
        private Dictionary<Uri, string[]> ProtocolsForProduct = new Dictionary<Uri, string[]>();


        public void SetProductSupportedProtocols(Uri productUri, params string[] protocols)
        {
            ProtocolsForProduct[productUri] = protocols;
        }

        public Task<string[]> SupportedProtocolsAsync(Uri productUri, string conversationId, Uri contextUri)
        {
            if (ProtocolsForProduct.ContainsKey(productUri))
            {
                return Task.FromResult(ProtocolsForProduct[productUri]);
            }

            return Task.FromResult(new string[]{UnsupportedProtocolType});
        }
    }
}
