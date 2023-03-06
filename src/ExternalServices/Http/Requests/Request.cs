using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Trainline.PromocodeService.ExternalServices.Http.Constants;

namespace Trainline.PromocodeService.ExternalServices.Http.Requests
{
    public static class Request
    {
        public static HttpRequestMessage CreateRequest(this HttpMethod httpMethod, Uri requestUri, string acceptHeader, string conversationId, string contextUri)
        {
            var request = new HttpRequestMessage(httpMethod, requestUri);
            request.Headers.Add(Headers.ConversationId, conversationId);
            request.Headers.Add(Headers.ContextUri, contextUri?.ToString());
            request.Headers.Add(Headers.UserAgent, UserAgentAccessor.GetUserAgent());
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader) { CharSet = null });

            return request;
        }
    }
}
