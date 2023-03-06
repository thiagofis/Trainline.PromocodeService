using System;
using System.Threading.Tasks;

namespace Trainline.PromocodeService.ExternalServices.Http.Requests
{
    public interface IHttpRequestClient
    {
        Task<HttpResult<TResponse>> PostAsync<TPayload, TResponse>(Uri uri, TPayload payload);
        Task<HttpResult<TResponse>> GetAsync<TResponse>(Uri uri);
    }
}
