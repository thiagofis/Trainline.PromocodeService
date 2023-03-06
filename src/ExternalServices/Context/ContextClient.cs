using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trainline.PromocodeService.ExternalServices.Context.Contract;
using Trainline.PromocodeService.ExternalServices.Context.Exceptions;
using Trainline.PromocodeService.ExternalServices.Exceptions;
using Trainline.PromocodeService.ExternalServices.Http.Requests;

namespace Trainline.PromocodeService.ExternalServices.Context
{
    public class ContextClient : IContextClient
    {
        private readonly IHttpRequestClient _httpRequestClient;
        private readonly ILogger<ContextClient> _logger;

        public ContextClient(IHttpRequestClient httpRequestClient, ILogger<ContextClient> logger)
        {
            _httpRequestClient = httpRequestClient;
            _logger = logger;
        }

        public async Task<ContextParameters> GetAsync(string contextUri)
        {
            try
            {
                var uri = new Uri(contextUri);
                var httpResult = await _httpRequestClient.GetAsync<ContextParameters>(uri);
                return httpResult.Result;
            }
            catch (HttpResponseException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Context was not found. ContextUri={ContextUri}", contextUri);
                    throw new ContextNotFoundException(contextUri);
                }

                throw;
            }
        }
    }
}
