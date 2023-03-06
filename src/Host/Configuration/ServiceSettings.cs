using System;
using System.Diagnostics;
using System.Reflection;

namespace Trainline.PromocodeService.Host.Configuration
{
    public class ServiceSettings
    {
        public string ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string EnvironmentName { get; set; } = Environment.GetEnvironmentVariable("TTL_ENVIRONMENT") ?? "Dev";
        public string Slice { get; set; } = Environment.GetEnvironmentVariable("TTL_SERVICE_SLICE") ?? "NA";
        public readonly Func<string> Version = () =>
        {
            var version = Environment.GetEnvironmentVariable("EM_SERVICE_VERSION");

            if(!string.IsNullOrEmpty(version)) {
                return version;
            }

            var codeAssembly = Assembly.GetExecutingAssembly();
            var info = FileVersionInfo.GetVersionInfo(codeAssembly.Location);
            return info.ProductVersion;
        };
    }
}
