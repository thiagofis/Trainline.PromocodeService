using Microsoft.Extensions.Logging;
using Trainline.HttpContextTracing;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Trainline.PromocodeService.Host.Logging
{
    public class HttpContextTraceLogger : IHttpContextTracer
    {
        private readonly ILogger<HttpContextTraceLogger> _logger;

        public HttpContextTraceLogger(ILogger<HttpContextTraceLogger> logger)
        {
            _logger = logger;
        }

        public void Trace(HttpContextTrace trace)
        {
            _logger.Log(GetLevelByStatusCode(trace.Response.StatusCode), "HttpRequestResponse: {@trace}", trace);
        }

        private static LogLevel GetLevelByStatusCode(int statusCode)
        {
            if (statusCode >= 500) return LogLevel.Error;
            if (statusCode >= 400) return LogLevel.Warning;

            return LogLevel.Information;
        }
    }
}
