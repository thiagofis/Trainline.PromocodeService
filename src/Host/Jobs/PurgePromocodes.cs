using Dapper;
using Microsoft.Extensions.Logging;
using Trainline.PromocodeService.Service.Repository;

namespace Trainline.PromocodeService.Host.Jobs
{
    public class PurgePromocodes : ICronJob
    {
        private readonly ILogger<PurgePromocodes> _logger;
        private readonly IDbConnectionFactory _connectionFactory;

        public string Cron => "0 6 * * *"; // Every Day at 6am


        public PurgePromocodes(ILogger<PurgePromocodes> logger, IDbConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public void Run()
        {
            _logger.LogInformation($"Running {this.GetType().Name}");

            var sql = $"DELETE FROM dbo.Promocodes where [RetentionDate] < GETDATE()";

            using (var connection = _connectionFactory.CreateOpenConnectionAsync())
            {
                connection.Result.Query(sql);
            }
        }
    }
}
