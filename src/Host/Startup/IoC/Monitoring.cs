using Microsoft.Extensions.DependencyInjection;
using Trainline.NewRelic.CustomAttributes;

namespace Trainline.PromocodeService.Host.Startup.IoC
{
    public static class Monitoring
    {
        public static IServiceCollection RegisterMonitoring(this IServiceCollection services)
        {
            services.AddScoped<INewRelicMonitor, NewRelicMonitor>();

            return services;
        }
    }
}
