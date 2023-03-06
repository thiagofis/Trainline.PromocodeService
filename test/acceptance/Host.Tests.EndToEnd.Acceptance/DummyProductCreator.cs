

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance.Contracts;
using Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance.Contracts.JsonApi;

namespace Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance
{
    public class DummyProductCreator
    {
        public static async Task<Uri> Create(Uri productUri, Uri contextUri)
        {
            var product = new Contracts.Product
            {
                Data = new JsonApiDocument.DataElement<Contracts.Product.Attributes, Contracts.Product.Links, Contracts.Product.Relationships>
                {
                    Attributes = new Contracts.Product.Attributes
                    {
                        ListPrice = new ListPrice
                        {
                            Amount = 10.00m,
                            Breakdown =
                                new List<ListPriceBreakdownItem>
                                {
                                    new ListPriceBreakdownItem {Amount = 10m, Reference = "foo"}
                                },
                            CurrencyCode = "GBP"
                        }
                    },
                    Type = "product"
                }
            };

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, productUri))
                {
                    request.Headers.Add("ConversationId", Guid.NewGuid().ToString());
                    request.Headers.Add("ContextUri", contextUri.ToString());
                    request.Content = new StringContent(JsonConvert.SerializeObject(product),
                        Encoding.UTF8, "application/vnd.trainline.retailprotocol.v2+json");

                    using (var response = await client.SendAsync(request))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine(await response.Content.ReadAsStringAsync());
                        }
                        response.EnsureSuccessStatusCode();
                        return new Uri(JsonConvert.DeserializeObject<Host.Tests.EndToEnd.Acceptance.Contracts.Product>(await response.Content.ReadAsStringAsync())
                            .Data.Links.Self.Href);
                    }
                }
            }
        }
    }
}
