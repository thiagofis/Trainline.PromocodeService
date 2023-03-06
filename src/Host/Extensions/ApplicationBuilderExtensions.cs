using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Trainline.AspNetCore.HealthCheck;
using Trainline.AspNetCore.HealthCheck.Checks;
using Trainline.AspNetCore.HttpContextTracing.Extensions;
using Trainline.PromocodeService.Host.Exceptions;
using Trainline.PromocodeService.Host.Configuration;
using Trainline.HttpContextTracing;
using Trainline.NetCore.Exceptions.Extensions;
using Trainline.NetCore.StandardHeaders.Middleware;
using Trainline.NetStandard.Exceptions.Configuration;
using Trainline.NetStandard.Exceptions.Contracts;
using Trainline.NetStandard.StandardHeaders.Enums;
using Trainline.PromocodeService.Common;
using Trainline.PromocodeService.Common.Exceptions;
using Trainline.PromocodeService.ExternalServices.Context.Exceptions;
using Trainline.PromocodeService.ExternalServices.Customer.Exceptions;
using Trainline.PromocodeService.ExternalServices.Exceptions;
using Trainline.PromocodeService.ExternalServices.Voucherify;
using Trainline.PromocodeService.Host.Middleware;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.Exceptions;
using Error = Trainline.NetStandard.Exceptions.Contracts.Error;
using ErrorMessagePrefix = Trainline.PromocodeService.ExternalServices.Voucherify.Contract.ErrorMessagePrefix;
using Trainline.PromocodeService.ExternalServices.DiscountCard;
using Trainline.PromocodeService.ExternalServices.CardType;

namespace Trainline.PromocodeService.Host.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
        {
            var serviceSettings = app.ApplicationServices.GetService<IOptions<ServiceSettings>>().Value;

            var serviceInformation = GetServiceInformation(serviceSettings);

            return app.AddHealthChecks()
                .WithServiceInformation(serviceInformation)
                .WithRootEndpoint()
                .WithPingEndpoint()
                .WithDiagnosticsEndpoint()
                .DefineEndpoint("/diagnostics/installationcheck")
                .UseHealthChecks();
        }

        public static IApplicationBuilder AddMiddleware(this IApplicationBuilder app)
        {
            app
                .UseMiddleware(typeof(HeaderMiddleware), new object[] { DefaultHeaders.All })
                .UseMiddleware<HeaderLoggingMiddleware>();

            return app;
        }

        public static IApplicationBuilder SetupRequestTracing(this IApplicationBuilder app)
        {
            var httpContextTraceLoggers = app.ApplicationServices.GetServices<IHttpContextTracer>();

            app.AddHttpContextTracing(httpContextTraceLoggers.ToArray())
                .Use();

            return app;
        }

        public static IApplicationBuilder SetupSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
                options.RoutePrefix = "docs";
            });

            return app;
        }

        public static IApplicationBuilder SetupExceptionHandling(this IApplicationBuilder app)
        {
            app.UseExceptionHandlingMiddleware(new ExceptionMappingConfiguration()
                .For("PromocodeService")
                .Map<ValidationServiceException>(HttpStatusCode.BadRequest)
                .Map<VoucherifyException>(e => ExceptionMap.MapError((VoucherifyException)e), e => ExceptionMap.MapStatusCode((VoucherifyException)e))
                .Map<VoucherifyPromocodeNotFoundException>(e => new Error(Severity.Correctable, ErrorCodes.PromocodeNotFound,
                    ((VoucherifyPromocodeNotFoundException)e).Value), HttpStatusCode.NotFound)
                .MapToErrorResponse<PromocodeValidatorException>(((exception, s) =>
                    ExceptionMap.MapErrorResult((PromocodeValidatorException)exception, s)), HttpStatusCode.BadRequest)
                .Map<NotApplicableException>(e =>
                    new Error(Severity.Correctable, ((NotApplicableException)e).ErrorCode, ((NotApplicableException)e).Value), HttpStatusCode.BadRequest)
                .Map<QuantityExceededException>(e => new Error(Severity.Correctable, ErrorCodes.PromocodeAlreadyRedeemed, ""), HttpStatusCode.BadRequest)
                .Map<PromocodeNotFoundException>(e => new Error(Severity.Correctable, ErrorCodes.PromocodeNotFound, ""), HttpStatusCode.NotFound)
                .Map<AlreadyRolledBackException>(e => new Error(Severity.Correctable, ErrorCodes.RedemptionAlreadyReinstated, ""), HttpStatusCode.BadRequest)
                .Map<NotValidProductUriException>(e => new Error(Severity.Correctable, ((NotValidProductUriException)e).ErrorCode, ((NotValidProductUriException)e).Value), HttpStatusCode.BadRequest)
                .Map<PromocodeCurrencyNotApplicableException>(e => new Error(Severity.Correctable, ErrorCodes.PromocodeCurrencyNotApplicable, ((PromocodeCurrencyNotApplicableException)e).Value), HttpStatusCode.BadRequest)
                .Map<InvalidPromocodeException>(e => new Error(Severity.Correctable, ErrorCodes.InvalidPromocode, ((InvalidPromocodeException)e).Value), HttpStatusCode.BadRequest)
                .Map<RedemptionTotalLimitReachedException>(e => new Error(Severity.Correctable, ErrorCodes.RedemptionTotalLimitReached, ((RedemptionTotalLimitReachedException)e).Message), HttpStatusCode.BadRequest)
                .Map<PromocodeExpiredException>(e => new Error(Severity.Correctable, ErrorCodes.PromocodeExpired, e.Message), HttpStatusCode.BadRequest)
                .Map<CustomerNotNewException>(e => new Error(Severity.Correctable, ErrorCodes.PromocodeCustomerNotNew, e.Message), HttpStatusCode.BadRequest)
                .Map<VoucherifyRedeemException>(e =>
                {
                    var message = ((VoucherifyRedeemException)e).Error.Message;
                    var errorCode = ErrorCodes.GetErrorCode(ErrorMessagePrefix.GetPrefix((message)));
                    var value = ErrorMessagePrefix.GetValue(message);
                    return new Error(Severity.Correctable, errorCode, value);
                }, HttpStatusCode.BadRequest)
                .Map<CustomerIsNotEligibleForTheCampaignException>(e => new Error(Severity.Correctable, ErrorCodes.CustomerIsNotEligibleForTheCampaign, e.Message), HttpStatusCode.BadRequest)
                .Map<CustomerImplicitRegistrationException>(e => new Error(Severity.Correctable, ErrorCodes.CustomerImplicitRegistrationFailed, e.Message), HttpStatusCode.BadRequest)
                .Map<ContextNotFoundException>(e => new Error(Severity.Correctable, ErrorCodes.ContextNotFound, e.Message), HttpStatusCode.BadRequest)
                .Map<HttpResponseException>(e => new Error(Severity.Unexpected, ErrorCodes.ThirdPartyServiceUnavailableOrNotWorkingAsExpected, e.Message),HttpStatusCode.ServiceUnavailable)
                .Map<DiscountCardClientException>(e => ExceptionMap.MapError((DiscountCardClientException)e), e => ExceptionMap.MapStatusCode((DiscountCardClientException)e))
                .Map<CardTypeClientException>(e => ExceptionMap.MapError((CardTypeClientException)e), e => ExceptionMap.MapStatusCode((CardTypeClientException)e))
                );

            return app;
        }


        private static Dictionary<string, object> GetServiceInformation(ServiceSettings settings)
        {
            return new Dictionary<string, object>
            {
                {"ApplicationId", settings.ApplicationId},
                {"ServiceName", "PromocodeService"},
                {"Version", settings.Version()},
                {"Slice", settings.Slice}
            };
        }
    }
}
