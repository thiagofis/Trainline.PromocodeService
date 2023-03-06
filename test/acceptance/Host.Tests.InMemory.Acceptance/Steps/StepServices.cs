using Microsoft.Extensions.DependencyInjection;
using Trainline.PromocodeService.Acceptance.Helpers;
using Trainline.PromocodeService.Acceptance.Helpers.Steps;
using Trainline.PromocodeService.Service.Repository;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Steps
{
    public static class StepServices
    {
        public static void RegisterSteps(this IServiceCollection services)
        {
            services.AddSingleton(_ => new AutoFixture.Fixture());
            services.AddSingleton<IPromocodeRepository, InMemoryPromocodeRepository>();
            services.AddSingleton<IRedemptionRepository, InMemoryRedemptionRepository>();
            services.AddSingleton<ILedgerRepository, InMemoryLedgerRepository>();
            services.AddSingleton<ICampaignRepository, InMemoryCampaignRepository>();
            services.AddScoped<InMemoryGivenSteps>();
            services.AddScoped<WhenSteps>();
            services.AddScoped<ThenSteps>();
        }
    }
}
