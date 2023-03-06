using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Trainline.PromocodeService.Host.Configuration;
using Trainline.PromocodeService.Host.Extensions;
using Trainline.PromocodeService.Host.Jobs;
using Trainline.PromocodeService.Host.Startup.IoC;
using Trainline.PromocodeService.Host.Migrations;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.Repository;

namespace Trainline.PromocodeService.Host.Startup
{
    public static class IoCRegistration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            services.AddSingleton<DatabaseMigrator>();

            services.RegisterVortex(hostingEnvironment, configuration);

            services
                .RegisterSwagger()
                .RegisterConfiguration(configuration)
                .RegisterLogging(configuration)
                .RegisterMiddleware()
                .RegisterExternalServices()
                .RegisterCircuitBreakerPolicies()
                .RegisterRepositories()
                .RegisterMappers(configuration)
                .RegisterUrlHelper()
                .RegisterMonitoring()
                .RegisterServices();


            if (!hostingEnvironment.IsInMemoryTests())
            {
                services.RegisterHangFire(configuration);
            }

            //TODO: Register any services here
            return services;
        }
    }
}
