using Microsoft.Extensions.DependencyInjection;
using Trainline.PromocodeService.ExternalServices;
using Trainline.PromocodeService.ExternalServices.CardType;
using Trainline.PromocodeService.ExternalServices.Context;
using Trainline.PromocodeService.ExternalServices.Customer;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute;
using Trainline.PromocodeService.ExternalServices.DiscountCard;
using Trainline.PromocodeService.ExternalServices.Http.Handlers;
using Trainline.PromocodeService.ExternalServices.Http.Requests;
using Trainline.PromocodeService.ExternalServices.TravelProduct;
using Trainline.PromocodeService.ExternalServices.Voucherify;
using Trainline.PromocodeService.Service;

namespace Trainline.PromocodeService.Host.Startup.IoC
{
    public static class ExternalServices
    {
        public static IServiceCollection RegisterExternalServices(this IServiceCollection services)
        {
            services.AddHttpClient<IDiscountCardClient, DiscountCardClient>()
                .AddPolicyHandlerFromRegistry(CircuitBreakers.Standard);

            services.AddHttpClient<ICardTypeClient, CardTypeClient>()
                .AddPolicyHandlerFromRegistry(CircuitBreakers.Standard);

            services.AddHttpClient<IVoucherifyClient, VoucherifyClient>()
                .AddPolicyHandlerFromRegistry(CircuitBreakers.Standard);

            services.AddHttpClient<IHttpRequestClient, StandardHttpRequestClient>()
                .AddHttpMessageHandler<DefaultHeadersHandler>()
                .AddPolicyHandlerFromRegistry(CircuitBreakers.Standard);

            services.AddScoped<ITravelProductClient, TravelProductClient>();
            services.AddScoped<DefaultHeadersHandler>();
            services.AddScoped<IContextClient, ContextClient>();
            services.AddScoped<ICustomerServiceClient, CustomerServiceClient>();
            services.AddScoped<ICustomerAttributeClient, CustomerAttributeClient>();

            return services;
        }
    }
}
