using System;
using Microsoft.Extensions.Logging;
using Trainline.VortexPublisher.Logging;

namespace Trainline.PromocodeService.Host.Logging
{
    public class VortexLogger : IVortexLogger
    {
        private readonly ILogger<VortexLogger> _logger;

        public VortexLogger(ILogger<VortexLogger> logger)
        {
            _logger = logger;
        }

        public void Log(VortexPublisher.Logging.LogLevel logLevel, string message, Exception exception = null)
        {
            switch (logLevel)
            {
                case VortexPublisher.Logging.LogLevel.Information:
                    _logger.LogInformation(exception, message);
                    break;
                case VortexPublisher.Logging.LogLevel.Error:
                    _logger.LogError(exception, message);
                    break;
                case VortexPublisher.Logging.LogLevel.Critical:
                    _logger.LogCritical(exception, message);
                    break;
                default:
                    _logger.LogDebug(exception, message);
                    break;
            }
        }
    }
}
