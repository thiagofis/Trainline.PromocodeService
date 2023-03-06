using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Trainline.PromocodeService.Acceptance.Helpers
{
    public class ContextUriCreator
    {
        public static Uri Create(Uri contextRequestUri)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, contextRequestUri)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        Values = new Dictionary<string, string[]>
                        {
                            {"Application:ManagedGroupId", new []{"20"}},
                            {"Application:EntryPointId", new []{"999"}},
                            {"Application:User:IpAddress", new []{"0.0.0.0"}},
                        }
                    }), Encoding.UTF8, "application/json")
                };
                request.Headers.Add("ConversationId", Guid.NewGuid().ToString());

                var response = httpClient.SendAsync(request).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                }
                return response.Headers.Location;
            }
        }
    }
}
