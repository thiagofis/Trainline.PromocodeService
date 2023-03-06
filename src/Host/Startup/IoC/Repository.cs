using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Trainline.PromocodeService.Host.Configuration;
using Trainline.PromocodeService.Service.Repository;

namespace Trainline.PromocodeService.Host.Startup.IoC
{
    public static class Repository
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddTransient(p => (IDbConnectionFactory)new DbConnectionFactory(p.GetService<IOptions<DatabaseSettings>>().Value.ConnectionString));
            services.AddSingleton<IPromocodeRepository, PromocodeRepository>();
            services.AddSingleton<IRedemptionRepository, RedemptionRepository>();
            services.AddSingleton<ILedgerRepository, LedgerRepository>();
            services.AddSingleton<ICampaignRepository, CampaignRepository>();

            return services;
        }
    }
}
