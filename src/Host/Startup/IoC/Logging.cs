using System;
using System.Reflection;
using Destructurama;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.AspNetCore;
using Trainline.Extensions.Logging.Core;
using Trainline.Extensions.Logging.Providers.Serilog;
using Trainline.HttpContextTracing;
using Trainline.PromocodeService.Host.Logging;

namespace Trainline.PromocodeService.Host.Startup.IoC
{
    public static class Logging
    {
        public static IServiceCollection RegisterLogging(this IServiceCollection services, IConfiguration configuration)
        {
            services.Scan(s => s
                .FromAssemblies(Assembly.GetAssembly(typeof(HttpContextTraceLogger)))
                .AddClasses(c => c.AssignableTo(typeof(IHttpContextTracer)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

            services.AddTrainlineLoggingWithWorkingJsonNetTypes(configuration, options =>
            {
                options.LogOriginalIp = true;
                options.ConsoleAsJson = true;
            });

            return services;
        }

        private static IServiceCollection AddTrainlineLoggingWithWorkingJsonNetTypes(this IServiceCollection services, IConfiguration configuration, Action<LoggingConfiguration> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddTrainlineLoggingWithWorkingJsonNetTypes(configuration);
            services.Configure(configureOptions);

            return services;
        }

        private static IServiceCollection AddTrainlineLoggingWithWorkingJsonNetTypes(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Configure<LoggingConfiguration>(configuration);

            services.AddSingleton(s =>
            {
                var options = s.GetRequiredService<IOptions<LoggingConfiguration>>();
                var loggerConfiguration = new LoggerConfiguration();
                var configurationRoot = s.GetRequiredService<IConfiguration>();
                loggerConfiguration.Configure(configurationRoot, options.Value);
                loggerConfiguration.Destructure.JsonNetTypes();
                var logger = loggerConfiguration.CreateLogger();
                return (ILoggerFactory)new SerilogLoggerFactory(logger, true);
            });

            return services;
        }

    }
}
