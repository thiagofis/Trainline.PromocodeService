using System;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trainline.PromocodeService.Host.Configuration;
using Trainline.PromocodeService.Host.Jobs;
using Trainline.VortexPublisher.Resilience.Hangfire;

namespace Trainline.PromocodeService.Host.Startup.IoC
{
    public static class Hangfire
    {
        public static IServiceCollection RegisterHangFire(this IServiceCollection services, IConfiguration configuration)
        {

            var databaseSettings = configuration.GetSection("database").Get<DatabaseSettings>();
            services.AddHangfire(x => x.UseSqlServerStorage(databaseSettings.ConnectionString));
            services.AddHangfireServer();

            services.AddSingleton<ICronJob, PurgePromocodes>();

            return services;
        }

        public static void UseHangfire(this IApplicationBuilder app, IRecurringJobManager jobManager,
            IConfiguration configuration)
        {
            app.UseHangfireDashboard("/hangfire",
                new DashboardOptions { Authorization = new[] { new AnonymousDashboardAuthFilter() } });

            foreach (var job in app.ApplicationServices.GetServices<ICronJob>())
            {
                jobManager.AddOrUpdate(job.GetType().Name, () => job.Run(), job.Cron);
            }
        }
    }
}
