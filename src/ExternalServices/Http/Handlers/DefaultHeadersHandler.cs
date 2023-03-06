using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Trainline.NetStandard.StandardHeaders.Extensions;
using Trainline.NetStandard.StandardHeaders.Services;
using Trainline.PromocodeService.ExternalServices.Http.Constants;

namespace Trainline.PromocodeService.ExternalServices.Http.Handlers
{
    public class DefaultHeadersHandler : DelegatingHandler
    {
        private readonly IHeaderService _headerService;

        public DefaultHeadersHandler(IHeaderService headerService)
        {
            _headerService = headerService;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.Add(Headers.ContextUri, _headerService.GetContextUri());
            request.Headers.Add(Headers.ConversationId, _headerService.GetConversationId());

            return base.SendAsync(request, cancellationToken);
        }
    }
}
