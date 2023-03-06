using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Service.Repository
{
    public class RedemptionRepository : IRedemptionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RedemptionRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ICollection<Redemption>> GetByPromocodeId(string promocodeId)
        {
            var sql = $"SELECT [Id], [PromocodeId], [RedemptionId] FROM dbo.Redemptions WHERE [PromocodeId]=@promoCodeId";

            using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            return (await connection.QueryAsync<Redemption>(sql, new { promoCodeId = promocodeId })).ToList();
        }

        public async Task<Redemption> Get(string promocodeId, string redemptionId)
        {
            var sql = $"SELECT [Id], [PromocodeId], [RedemptionId] FROM dbo.Redemptions WHERE [RedemptionId]=@redemptionId AND [PromocodeId]=@promocodeId";

            using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            return (await connection.QuerySingleAsync<Redemption>(sql, new
            {
                redemptionId = redemptionId,
                promoCodeId = promocodeId
            }));
        }

        public async Task<Redemption> Add(Redemption redemption)
        {
            var sql = $"INSERT INTO dbo.Redemptions ([PromocodeId], [RedemptionId]) " +
                      "OUTPUT INSERTED.Id " +
                      "VALUES (@PromocodeId, @RedemptionId)";

            using (var connection = await _connectionFactory.CreateOpenConnectionAsync())
            {

                var id = connection.QuerySingle<int>(sql, redemption);
                redemption.Id = id;
            }

            return redemption;
        }
    }
}
