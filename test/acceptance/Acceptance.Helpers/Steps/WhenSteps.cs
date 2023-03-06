using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Trainline.Acceptance;
using Trainline.Acceptance.Response;
using Trainline.PromocodeService.Acceptance.Helpers;
using Trainline.PromocodeService.Contract;

namespace Trainline.PromocodeService.Acceptance.Helpers.Steps
{
    public class WhenSteps
    {
        private readonly HostTestContext _context;

        public WhenSteps(HostTestContext context)
        {
            _context = context;
        }

        public Task<ApiResponse<T>> IGet<T>(Uri location, RequestOptions options = null)
            => IGet<T>(location.ToString(), options);

        public Task<ApiResponse<T>> IGet<T>(string location, RequestOptions options = null)
        {
            options = options ?? new RequestOptions();

            SetHeaders(options);

            return _context.ApiClient.GetAsync<T>(location);
        }

        public Task<ApiResponse<TResponse>> IPost<TResponse>(Uri location, RequestOptions options = null)
            => IPost<TResponse>(location.ToString(), options);

        public Task<ApiResponse<TResponse>> IPost<TResponse>(string location, RequestOptions options = null)
        {
            return Send<object, TResponse>(null, location, HttpMethod.Post, options);
        }

        public Task<ApiResponse<TResponse>> IPost<TRequest, TResponse>(TRequest request, Uri location,
            RequestOptions options = null)
            => IPost<TRequest, TResponse>(request, location.ToString(), options);

        public Task<ApiResponse<TResponse>> IPost<TRequest, TResponse>(TRequest request, string location,
            RequestOptions options = null)
        {
            return Send<TRequest, TResponse>(request, location, HttpMethod.Post, options);
        }

        private Task<ApiResponse<TResponse>> Send<TResponse>(string location, HttpMethod method, RequestOptions options)
        {
            return Send<object, TResponse>(null, location, method, options);
        }

        private Task<ApiResponse<TResponse>> Send<TRequest, TResponse>(TRequest request, string location, HttpMethod method, RequestOptions options)
        {
            return Send<TRequest, TResponse>(request, location, method, options, Enumerable.Empty<(string, string)>());
        }

        private Task<ApiResponse<TResponse>> Send<TRequest, TResponse>(TRequest request, string location, HttpMethod method, RequestOptions options, IEnumerable<(string, string)> additionalHeaders)
        {
            options = options ?? new RequestOptions();

            var contentNegotiation = GetContentNegotiation<TResponse>(options, additionalHeaders);

            return _context.ApiClient.SendRequest<object, TResponse>(method, location, request, contentNegotiation);
        }

        private ContentNegotiation GetContentNegotiation<TResponse>(RequestOptions options, IEnumerable<(string Name, string Value)> additionalHeaders)
        {
            var contentNegotiation = new ContentNegotiation
            {
                ContentType = options.ContentType
            };

            if (options.IncludeContextUriHeader)
            {
                contentNegotiation.Headers["ContextUri"] = _context.ContextUri.ToString();
            }
            if (options.IncludeConversationIdHeader)
            {
                contentNegotiation.Headers["ConversationId"] = _context.ConversationId;
            }
            if (options.IncludeUserAgentHeader)
            {
                contentNegotiation.Headers["user-agent"] = "user-agent-value";
            }
            if (options.Accept != null)
            {
                contentNegotiation.Headers["Accept"] = options.Accept;
            }

            foreach (var header in additionalHeaders)
            {
                contentNegotiation.Headers[header.Name] = header.Value;
            }

            return contentNegotiation;
        }

        private void SetHeaders(RequestOptions options)
        {
            if (options.IncludeContextUriHeader)
            {
                _context.ApiClient.SetContextUriHeader("http://contexturi.com");
            }
            if (options.IncludeConversationIdHeader)
            {
                _context.ApiClient.SetConversationIdHeader(Guid.NewGuid().ToString());
            }
            if (options.IncludeUserAgentHeader)
            {
                _context.ApiClient.SetDefaultRequestHeaders("user-agent", "user-agent-value");
            }
        }

        public Task<ApiResponse<TResponse>> IMake<TResponse>(Request<IEnumerable<Invoice>> request)
        {
            return Send<IEnumerable<Invoice>, TResponse>(request.Content, request.Location, request.Method, request.Options, request.Headers);
        }
    }

    public class Request<TRequest>
    {
        public TRequest Content { get; set; }
        public HttpMethod Method { get; set; }
        public string Location { get; set; }
        public RequestOptions Options { get; set; }
        public IEnumerable<(string Name, string Value)> Headers { get; set; } = Enumerable.Empty<(string, string)>();

        public Request<TRequest> WithHeaders(params (string Name, string Value)[] headers)
        {
            return new Request<TRequest>
            {
                Content = Content,
                Method = Method,
                Location = Location,
                Headers = Headers.Concat(headers)
            };
        }
    }
}
