using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Trainline.PromocodeService.Host.Tests.EndToEnd.Acceptance
{
    public static class ConfigurationService
    {
        public static AppSettings Load()
        {
            var configurationRoot = LoadConfigurationRoot();

            return configurationRoot.Get<AppSettings>();
        }

        private static IConfigurationRoot LoadConfigurationRoot()
        {
            var environment = Environment.GetEnvironmentVariable("DEPLOYMENT_ENV_NAME") ?? "Development";

            return new ConfigurationBuilder()
                .AddJsonFile("./appsettings.json", false)
                .AddJsonFile($"./appsettings.{environment}.json", true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
