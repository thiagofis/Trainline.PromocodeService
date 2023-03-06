using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Trainline.ConventionalDeploy.Integration;
using Microsoft.Extensions.DependencyInjection;
using Trainline.PromocodeService.Host.Migrations;
using Microsoft.Extensions.Logging;

namespace Trainline.PromocodeService.Host
{
    public class Program
    {
        private static IServiceContext _service;

        public static async Task Main(string[] args)
        {
            _service = TrainlineHosting.InitializeService(args);

            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            var host = CreateHostBuilder(args).Build();
            //disable migrations until the db is created
            var logger = host.Services.GetService<ILogger<Program>>();

            var migrator = host.Services.GetService<DatabaseMigrator>();
            try
            {
                migrator.Run();
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Migrations failed");
                throw;
            }
            logger.LogInformation("Migration completed. Host run starting");

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);

# if IS_WINDOWS
            if (_service?.RunAsService ?? false) host.UseWindowsService();
# endif
            host.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json");
                    if (IsRunningLocally)
                    {
                        config.AddJsonFile("appsettings.Local.json");
                    }

                    config.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(opts => { });
                    webBuilder.UseStartup<Startup.Startup>();
                });

            return host;
        }

        private static bool IsRunningLocally => Environment.GetEnvironmentVariable("TTL_ENVIRONMENT") == null;
    }
}
