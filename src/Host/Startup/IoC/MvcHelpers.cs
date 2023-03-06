using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Trainline.PromocodeService.Host.Startup.IoC
{
    public static class MvcHelpers
    {
        public static IServiceCollection RegisterUrlHelper(this IServiceCollection services)
        {
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddScoped<IUrlHelper>(x => {
                    var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                    var factory = x.GetRequiredService<IUrlHelperFactory>();
                    return factory.GetUrlHelper(actionContext);
            });

            return services;
        }
    }
}
