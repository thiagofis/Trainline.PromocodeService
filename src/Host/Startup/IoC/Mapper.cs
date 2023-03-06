using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.Mappers;

namespace Trainline.PromocodeService.Host.Startup.IoC
{
    public static class Mapper
    {
        public static IServiceCollection RegisterMappers(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddScoped<IPromocodeMapper, PromocodeMapper>()
                .AddScoped<IVoucherifyMapper, VoucherifyMapper>()
                .AddSingleton<IRedemptionMapper, RedemptionMapper>()
                .AddSingleton<ILedgerMapper, LedgerMapper>()
                .AddScoped<Mappers.IPromocodeMapper, Mappers.PromocodeMapper>()
                .AddScoped<Mappers.IInvoiceMapper, Mappers.InvoiceMapper>()
                .AddScoped<Mappers.IRedemptionMapper, Mappers.RedemptionMapper>()
                .AddScoped<IValidationRuleMapper, ValidationRuleMapper>()
                .AddScoped<Mappers.ILedgerMapper, Mappers.LedgerMapper>();


            var customerAttributeSettings = configuration.GetSection("CustomerAttribute").Get<CustomerAttributeSettings>();

            if (customerAttributeSettings != null && customerAttributeSettings.IsServiceEnabled)
            {
                services.AddScoped<ICustomerVoucherifyMapper, CustomerVoucherifyMapper>();
            }
            else
            {
                services.AddScoped<ICustomerVoucherifyMapper, DefaultCustomerVoucherifyMapper>();
            }

            return services;
        }
    }
}
