using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Trainline.PromocodeService.Host.Configuration;

namespace Trainline.PromocodeService.Host.Startup.IoC
{
    public static class Swagger
    {
        public static IServiceCollection RegisterSwagger(this IServiceCollection services)
        {
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, Swagger.ConfigureSwaggerOptions>();
            services.AddSwaggerGen(options => options.OperationFilter<Swagger.SwaggerDefaultValues>());
            return services;
        }

        public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
        {
            private readonly IApiVersionDescriptionProvider _provider;

            public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

            public void Configure(SwaggerGenOptions options)
            {
                // add a swagger document for each discovered API version
                // note: you might choose to skip or document deprecated API versions differently
                foreach (var description in _provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                    options.OperationFilter<RequiredHeadersFilter>();
                }
            }

            private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
            {
                var info = new OpenApiInfo()
                {
                    Title = "Sample API",
                    Version = description.ApiVersion.ToString(),
                };

                if (description.IsDeprecated)
                {
                    info.Description += " This API version has been deprecated.";
                }

                return info;
            }
        }

        public class SwaggerDefaultValues : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var apiDescription = context.ApiDescription;
                operation.Deprecated |= apiDescription.IsDeprecated();

                if (operation.Parameters == null)
                    return;

                // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
                // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
                foreach (var parameter in operation.Parameters)
                {
                    var description = apiDescription.ParameterDescriptions.FirstOrDefault(p => p.Name == parameter.Name);
                    if (description == null)
                    {
                        continue;
                    }

                    if (parameter.Description == null)
                    {
                        parameter.Description = description.ModelMetadata?.Description;
                    }

                    if (parameter.Schema.Default == null && description.DefaultValue != null)
                    {
                        parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
                    }

                    parameter.Required |= description.IsRequired;
                }
            }
        }
    }
}
