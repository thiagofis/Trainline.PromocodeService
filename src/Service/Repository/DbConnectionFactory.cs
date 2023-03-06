using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Trainline.PromocodeService.Service.Repository
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<System.Data.IDbConnection> CreateOpenConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            return connection;
        }
    }
}
