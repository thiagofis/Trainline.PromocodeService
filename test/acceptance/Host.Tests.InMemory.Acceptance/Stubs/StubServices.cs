using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Trainline.Product.SupportedProtocols;
using Trainline.PromocodeService.Acceptance.Helpers;
using Trainline.PromocodeService.ExternalServices.CardType;
using Trainline.PromocodeService.ExternalServices.Customer;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute;
using Trainline.PromocodeService.ExternalServices.DiscountCard;
using Trainline.PromocodeService.ExternalServices.TravelProduct;
using Trainline.PromocodeService.ExternalServices.Voucherify;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.Repository;
using Trainline.VortexPublisher.EventPublishing;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs
{
    public static class StubServices
    {
        public static void RegisterStubs(this IServiceCollection services)
        {
            services.AddSingleton<HostTestContext>();

            services.AddSingleton<InMemoryVoucherifyClient>()
                .AddSingleton<IVoucherifyClient>(services => services.GetRequiredService<InMemoryVoucherifyClient>());

            services.AddSingleton<InMemoryPromocodeRepository>()
                .AddSingleton<IPromocodeRepository>(services => services.GetRequiredService<InMemoryPromocodeRepository>());

            services.AddSingleton<InMemoryLedgerRepository>()
                .AddSingleton<ILedgerRepository>(services => services.GetRequiredService<InMemoryLedgerRepository>());

            services.AddSingleton<InMemoryCampaignRepository>()
                .AddSingleton<ICampaignRepository>(services => services.GetRequiredService<InMemoryCampaignRepository>());

            services.AddSingleton<InMemoryCustomerServiceClient>()
                .AddSingleton<ICustomerServiceClient>(services => services.GetRequiredService<InMemoryCustomerServiceClient>());

            services.AddSingleton<InMemoryCustomerAttributeClient>()
                .AddSingleton<ICustomerAttributeClient>(services => services.GetRequiredService<InMemoryCustomerAttributeClient>());

            services.AddSingleton<InMemorySupportedProtocolsService>()
                .AddSingleton<ISupportedProtocolsService>(services => services.GetRequiredService<InMemorySupportedProtocolsService>());

            services.AddSingleton<InMemoryDiscountCardService>()
                .AddSingleton<IDiscountCardClient>(services => services.GetRequiredService<InMemoryDiscountCardService>());

            services.AddSingleton<InMemoryCardTypeClient>()
                .AddSingleton<ICardTypeClient>(services => services.GetRequiredService<InMemoryCardTypeClient>());

            services.AddSingleton<InMemoryTravelProductClient>()
               .AddSingleton<ITravelProductClient>(services => services.GetRequiredService<InMemoryTravelProductClient>());

            services.AddSingleton<IVortexEventPublisher, FakeVortexEventPublisher>();
            services.AddSingleton<IRecurringJobManager, FakeRecurringJobManager>();
        }
    }
}
