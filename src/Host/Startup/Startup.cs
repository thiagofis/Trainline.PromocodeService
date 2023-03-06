using System.Net;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Trainline.PromocodeService.Host.ActionFilters;
using Trainline.PromocodeService.Host.Extensions;
using Trainline.NetCore.StandardHeaders.ActionFilters;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Trainline.PromocodeService.Host.Startup.IoC;

namespace Trainline.PromocodeService.Host.Startup
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureContainerBuilder(services);
        }

        //Used to allow in-memory tests to add extra configuration to the service collection
        protected virtual void ConfigureContainerBuilder(IServiceCollection services)
        {
            services
                .AddOptions()
                .AddResponseCompression(options => { options.EnableForHttps = true; })
                .Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true)
                .AddHttpContextAccessor()
                .RegisterServices(_environment, _configuration)
                .AddControllers(options =>
                {
                    options.Filters.Add<ContractValidationAttribute>();
                    options.Filters.Add<ConversationIdAndContextUriRequiredAttribute>();
                    options.Filters.Add<UserAgentRequiredAttribute>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                    opts.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new TrainlineMediaTypeApiVersionReader();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRecurringJobManager jobManager, IApiVersionDescriptionProvider provider)
        {
            app.UseTrainlineLogging();

            if (env.IsProduction() || env.IsStaging())
            {
                app.UseHsts()
                    .UseHttpsRedirection();
            }

            if (!env.IsInMemoryTests())
            {
                //Add anything you don't want running when the in-memory tests are running
                app.UseHangfire(jobManager, _configuration);
            }

            app.UseRouting()
                .SetupSwagger(provider)
                .AddMiddleware()
                .UseHealthChecks()
                .SetupRequestTracing()
                .SetupExceptionHandling()
                .UseEndpoints(endpoints => endpoints.MapControllers());

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }
    }
}
