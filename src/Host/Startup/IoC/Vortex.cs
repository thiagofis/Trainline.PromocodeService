using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Trainline.PromocodeService.Host.Configuration;
using Trainline.PromocodeService.Host.Logging;
using Trainline.VortexPublisher.Configuration;
using Trainline.VortexPublisher.Resilience.Hangfire.Configuration;
using Trainline.VortexPublisher.Serialization.JsonDotNet;
using Microsoft.AspNetCore.Hosting;
using Trainline.PromocodeService.Host.Extensions;

namespace Trainline.PromocodeService.Host.Startup.IoC
{
    public static class Vortex
    {
        public static IServiceCollection RegisterVortex(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
        {
            if (environment.IsInMemoryTests()) return services;

            var databaseSettings = configuration.GetSection("database").Get<DatabaseSettings>();
            var serviceSettings = configuration.GetSection("service").Get<ServiceSettings>();
            var vortexSettings = configuration.GetSection("vortex").Get<VortexSettings>();

            services.AddSingleton(pf =>
            {
                var logger = pf.GetService<ILogger<VortexLogger>>();

                var vortexBuilder = VortexBuilderFactory.Configure()
                    .WithClientIdentity(identity =>
                    {
                        identity.ApplicationRegisterId = int.Parse(serviceSettings.ApplicationId);
                        identity.EnvironmentName = serviceSettings.EnvironmentName;
                        identity.ServiceName = serviceSettings.ApplicationName;
                    })
                    .WithMessagePublishEndpoint(vortexSettings.PublishUri.ToString())
                    .AndSerializer(new JsonDotNetSerializer())
                    .WithContextExpansionBehaviour(ContextExpansionBehaviour.AutoExpand)
                    .WithDefaultStream(vortexSettings.Stream)
                    .AndLogger(new VortexLogger(logger))
                    .WithHangfire(hf =>
                        hf.WithRetryLimit(10)
                            .WithJobStorage(HangfireJobStorage.SqlServer)
                           .WithSqlConnection(databaseSettings.ConnectionString)
                           .WithSliceName(serviceSettings.Slice));

                return vortexBuilder.CreateVortexEventPublisher();
            });

            return services;
        }
    }
}
