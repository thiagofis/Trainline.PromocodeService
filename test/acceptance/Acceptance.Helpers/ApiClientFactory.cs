using System.Net.Http;
using Trainline.Acceptance;
using Trainline.Acceptance.TraceWriter;

namespace Trainline.PromocodeService.Acceptance.Helpers
{
    public static class ApiClientFactory
    {
        public static ApiClient For(HttpClient client)
        {
            var apiClientConfiguration = new ApiClientConfiguration { TraceWriter = new ConsoleTraceWriter() };

            var apiClient = new ApiClient(client, apiClientConfiguration);
            return apiClient;
        }
    }
}
