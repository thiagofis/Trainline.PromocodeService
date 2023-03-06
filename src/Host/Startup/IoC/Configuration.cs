using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trainline.PromocodeService.ExternalServices.Customer;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute;
using Trainline.PromocodeService.ExternalServices.Voucherify;
using Trainline.PromocodeService.Host.Configuration;

namespace Trainline.PromocodeService.Host.Startup.IoC
{
    public static class Configuration
    {
        public static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ServiceSettings>(configuration.GetSection("Service"));
            services.Configure<SecuritySettings>(configuration.GetSection("Security"));
            services.Configure<DatabaseSettings>(configuration.GetSection("Database"));
            services.Configure<VortexSettings>(configuration.GetSection("Vortex"));
            services.Configure<VoucherifySettings>(configuration.GetSection("Voucherify"));
            services.Configure<CircuitBreakerSettings>(configuration.GetSection("CircuitBreaker"));
            services.Configure<CustomerServiceSettings>(configuration.GetSection("CustomerService"));
            services.Configure<CustomerAttributeSettings>(configuration.GetSection("CustomerAttribute"));

            return services;
        }
    }
}
