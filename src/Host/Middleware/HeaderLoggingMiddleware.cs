using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trainline.NetStandard.StandardHeaders.Enums;
using Trainline.NetStandard.StandardHeaders.Extensions;
using Trainline.NetStandard.StandardHeaders.Services;
using Trainline.NewRelic.CustomAttributes;

namespace Trainline.PromocodeService.Host.Middleware
{
    public class HeaderLoggingMiddleware
    {
        private static readonly List<string> IgnoredPathPrefixes = new List<string> { "/diagnostics", "/ping" };

        private readonly RequestDelegate _next;

        public HeaderLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ILogger<HeaderLoggingMiddleware> logger, INewRelicMonitor monitor, IHeaderService headerService)
        {
            if (IgnoredPathPrefixes.Any(p => httpContext.Request.Path.Value.StartsWith(p)))
            {
                await _next.Invoke(httpContext);
            }
            else
            {
                using (LogHeaders(logger, monitor, headerService))
                {
                    await _next.Invoke(httpContext);
                }
            }
        }

        private static IDisposable LogHeaders(ILogger<HeaderLoggingMiddleware> logger, INewRelicMonitor monitor, IHeaderService headerService)
        {
            var logScope = new Dictionary<string, string>();

            MonitorHeaderIfPresent(DefaultHeaders.ConversationId);
            MonitorHeaderIfPresent(DefaultHeaders.ContextUri);
            MonitorHeaderIfPresent(DefaultHeaders.UserAgent);
            MonitorHeaderIfPresent(CommonHeaders.XForwardedFor);

            return logger.BeginScope(logScope);

            void MonitorHeaderIfPresent(string headerName)
            {
                var headerValue = headerService.GetHeader(headerName).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(headerValue))
                {
                    logScope[headerName] = headerValue;
                    monitor.AddParameter(headerName, headerValue);
                }
            }
        }
    }
}
