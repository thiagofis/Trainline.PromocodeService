using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Trainline.Product.SupportedProtocols;
using Trainline.Product.SupportedProtocols.Logging;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.ExternalServices.DiscountCard;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.Caches;
using Trainline.PromocodeService.Service.DiscountStrategies;
using ValidationRule = Trainline.PromocodeService.Service.Repository.Entities.ValidationRule;

namespace Trainline.PromocodeService.Host.Startup.IoC
{
    public static class Service
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IValidationRuleService, ValidationRuleService>();
            services.AddScoped<IPromocodeService, PromocodeService.Service.PromocodeService>();
            services.AddSingleton<ISupportedProtocolsLogger, DiagnosticDebugLogger>();
            services.AddSingleton<ISupportedProtocolsService, Trainline.Product.SupportedProtocols.SupportedProtocolsService>();
            services.AddScoped<IRedemptionService, PromocodeService.Service.RedemptionService>();
            services.AddScoped<ILedgerService, PromocodeService.Service.LedgerService>();

            services.AddSingleton<IPromocodeDiscountStrategy, PromocodeAmountDiscountStrategy>()
                .AddSingleton<IPromocodeDiscountStrategy, PromocodePercentageDiscountStrategy>()
                .AddScoped<IPromocodeDiscountStrategyFactory, PromocodeDiscountStrategyFactory>();

            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IPromocodeValidator, PromocodeValidator>();
            services.AddScoped<ICampaignValidator<NewCustomerCampaignApplication, NewCustomerCampaignEligibilityData>, NewCustomerCampaignValidator>();

            services.AddScoped<IProductTypeClient, ProductTypeClient>();
            services.AddScoped<IInvoiceGenerator, InvoiceGenerator>();

            services.AddSingleton<InMemoryCache<IList<ValidationRule>>>();

            return services;
        }
    }
}
