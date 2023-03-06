using System.Threading.Tasks;
using Dapper;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Service.Repository
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CampaignRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Campaign> Get(string campaignId)
        {
            var sql = $"SELECT [Id], [CampaignId], [Redeemable] FROM dbo.Campaigns WHERE [CampaignId]=@CampaignId";

            using (var connection = await _connectionFactory.CreateOpenConnectionAsync())
            {

                return await connection.QueryFirstOrDefaultAsync<Campaign>(sql, new { CampaignId = campaignId });
            }
        }

        public async Task<Campaign> Add(Campaign campaign)
        {
            var sql = $"INSERT INTO dbo.Campaigns ([CampaignId], [Redeemable]) " +
                      "OUTPUT INSERTED.Id " +
                      "VALUES (@CampaignId, @Redeemable)";

            using (var connection = await _connectionFactory.CreateOpenConnectionAsync())
            {
                connection.QuerySingle<Campaign>(sql, campaign);
            }

            return campaign;
        }

        public async Task<Campaign> Update(Campaign campaign)
        {
            var sql =
                $"UPDATE dbo.Campaigns " +
                $"SET [Redeemable]=@Redeemable " +
                $"WHERE [CampaignId]=@CampaignId";
            using (var connection = await _connectionFactory.CreateOpenConnectionAsync())
            {
                await connection.ExecuteAsync(sql, campaign);
            }

            return campaign;
        }
    }
}
