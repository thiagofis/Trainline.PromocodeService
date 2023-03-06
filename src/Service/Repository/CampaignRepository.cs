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

        public async Task<CampaignEntity> Get(string campaignId)
        {
            var sql = $"SELECT [Id], [CampaignId], [Redeemable] FROM dbo.Campaigns WHERE [CampaignId]=@CampaignId";

            using (var connection = await _connectionFactory.CreateOpenConnectionAsync())
            {

                return await connection.QueryFirstOrDefaultAsync<CampaignEntity>(sql, new { CampaignId = campaignId });
            }
        }

        public async Task<CampaignEntity> Add(CampaignEntity campaignEntity)
        {
            var sql = $"INSERT INTO dbo.Campaigns ([CampaignId], [Redeemable]) " +
                      "OUTPUT INSERTED.Id " +
                      "VALUES (@CampaignId, @Redeemable)";

            using (var connection = await _connectionFactory.CreateOpenConnectionAsync())
            {
                connection.QuerySingle<CampaignEntity>(sql, campaignEntity);
            }

            return campaignEntity;
        }

        public async Task<CampaignEntity> Update(CampaignEntity campaignEntity)
        {
            var sql =
                $"UPDATE dbo.Campaigns " +
                $"SET [Redeemable]=@Redeemable " +
                $"WHERE [CampaignId]=@CampaignId";
            using (var connection = await _connectionFactory.CreateOpenConnectionAsync())
            {
                await connection.ExecuteAsync(sql, campaignEntity);
            }

            return campaignEntity;
        }
    }
}
