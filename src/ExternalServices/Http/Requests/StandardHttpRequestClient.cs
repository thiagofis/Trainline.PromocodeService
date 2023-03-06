using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Trainline.PromocodeService.ExternalServices.Exceptions;
using Trainline.PromocodeService.ExternalServices.Http.Constants;

namespace Trainline.PromocodeService.ExternalServices.Http.Requests
{
    public class StandardHttpRequestClient : IHttpRequestClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StandardHttpRequestClient> _logger;

        public StandardHttpRequestClient(HttpClient httpClient, ILogger<StandardHttpRequestClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public Task<HttpResult<TResponse>> PostAsync<TPayload, TResponse>(Uri uri, TPayload payload)
        {
            return CallAsync<TPayload, TResponse>(HttpMethod.Post, uri, payload);
        }

        public Task<HttpResult<TResponse>> GetAsync<TResponse>(Uri uri)
        {
            return CallAsync<string, TResponse>(HttpMethod.Get, uri, null);
        }

        private async Task<HttpResult<TResponse>> CallAsync<TPayload, TResponse>(HttpMethod httpMethod, Uri uri, TPayload payload)
        {
            using (var request = new HttpRequestMessage(httpMethod, uri))
            {
                if (payload != null)
                {
                    var json = JsonConvert.SerializeObject(payload);
                    request.Content = new StringContent(json, Encoding.UTF8, MediaTypes.ApplicationJson);
                }

                using (var response = await _httpClient.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError("The request was not successful. HttpMethod={HttpMethod} Uri={Uri} StatusCode={StatusCode} ResponseContent={ResponseContent}", httpMethod, uri, response.StatusCode, responseContent);
                        throw new HttpResponseException(uri, response.StatusCode, responseContent);
                    }

                    return new HttpResult<TResponse>
                    {
                        Result = await response.Content.ReadAsAsync<TResponse>(),
                        Location = response.Headers?.Location,
                        MediaType = response.Content?.Headers?.ContentType?.MediaType
                    };
                }
            }
        }
    }
}
