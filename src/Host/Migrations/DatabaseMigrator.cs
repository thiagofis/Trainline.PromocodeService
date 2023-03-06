using System.Reflection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Trainline.PromocodeService.Host.Configuration;
using Trainline.VortexPublisher.Resilience.Hangfire;

namespace Trainline.PromocodeService.Host.Migrations
{
    public class DatabaseMigrator
    {
        private readonly Assembly _migrationsAssembly;
        private readonly IOptions<DatabaseSettings> _databaseSettings;
        private readonly IOptions<ServiceSettings> _serviceSettings;
        private readonly ILoggerFactory _loggerFactory;

        public DatabaseMigrator(IOptions<DatabaseSettings> databaseSettings, 
            IOptions<ServiceSettings> serviceSettings,
            ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _databaseSettings = databaseSettings;
            _serviceSettings = serviceSettings;
            _migrationsAssembly = Assembly.GetExecutingAssembly();
        }

        public void Run()
        {
            var migrator = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(r =>
                    r.AddSqlServer2014()
                        .WithGlobalConnectionString(_databaseSettings.Value.MigrationConnectionString)
                        .ScanIn(_migrationsAssembly).For.Migrations())
                .AddLogging(lb => lb.AddProvider(new MigratorLoggerProvider(_loggerFactory)))
                .Configure<RunnerOptions>(opt => opt.Tags = new[] {_serviceSettings.Value.EnvironmentName.ToLowerInvariant()})
                .BuildServiceProvider(false);

            using (var scope = migrator.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateUp();
            }

            HangfireDatabaseMigrator.EnsureHangfireSchemaIsCreated(HangfireMigrationConnectionString.FromValue(_databaseSettings.Value.MigrationConnectionString));
        }

        public class MigratorLoggerProvider : ILoggerProvider
        {
            private readonly ILoggerFactory _loggerFactory;

            public MigratorLoggerProvider(ILoggerFactory loggerFactory)
            {
                _loggerFactory = loggerFactory;
            }
            public void Dispose()
            {
            }

            public ILogger CreateLogger(string categoryName) => _loggerFactory.CreateLogger(categoryName);
        }
    }
}
