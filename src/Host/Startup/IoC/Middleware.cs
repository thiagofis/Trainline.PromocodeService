using Microsoft.Extensions.DependencyInjection;
using Trainline.NetStandard.StandardHeaders.Services;

namespace Trainline.PromocodeService.Host.Startup.IoC
{
    public static class Middleware
    {
        public static IServiceCollection RegisterMiddleware(this IServiceCollection services)
        {
            services.Add(new ServiceDescriptor(typeof(IHeaderService), typeof(SingletonHeaderService), ServiceLifetime.Scoped));
            return services;
        }
        
    }
}