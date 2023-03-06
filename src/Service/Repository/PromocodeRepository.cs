using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Service.Repository
{
    public class PromocodeRepository : IPromocodeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PromocodeRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Promocode> GetByPromocodeId(string promocodeId)
        {
            var sql = $"SELECT [Id], [Code], [ValidityStartDate], [ValidityEndDate], [RedemptionQuantity], [RedeemedQuantity], [DiscountType], [DiscountAmount], [CurrencyCode], [ValidationRuleId], [PromocodeId], [CampaignId], [ProductType], [CampaignName], [RetentionDate] FROM dbo.Promocodes " +
                            "WHERE [PromocodeId]=@PromocodeId";

            using (var connection = await _connectionFactory.CreateOpenConnectionAsync())
            {

                return await connection.QueryFirstOrDefaultAsync<Promocode>(sql, new { PromocodeId = promocodeId});
            }
        }

        public async Task<Promocode> GetByCode(string code)
        {
            var sql = $"SELECT [Id], [Code], [ValidityStartDate], [ValidityEndDate], [RedemptionQuantity], [RedeemedQuantity], [DiscountType], [DiscountAmount], [CurrencyCode], [ValidationRuleId], [PromocodeId], [CampaignId], [ProductType], [CampaignName], [RetentionDate] FROM dbo.Promocodes " +
                      "WHERE [Code]=@Code";

            using (var connection = await _connectionFactory.CreateOpenConnectionAsync())
            {

                return await connection.QueryFirstOrDefaultAsync<Promocode>(sql, new { Code = code });
            }
        }

        public async Task<Promocode> Add(Promocode promocode)
        {
            var sql = $"INSERT INTO dbo.Promocodes ([Code], [ValidityStartDate], [ValidityEndDate], [RedemptionQuantity], [RedeemedQuantity], [DiscountType], [DiscountAmount], [CurrencyCode], [ValidationRuleId], [PromocodeId], [RetentionDate], [CampaignId], [ProductType], [CampaignName]) " +
                      "OUTPUT INSERTED.Id " +
                      "VALUES (@Code, @ValidityStartDate, @ValidityEndDate, @RedemptionQuantity, @RedeemedQuantity, @DiscountType, @DiscountAmount, @CurrencyCode, @ValidationRuleId, @PromocodeId, @RetentionDate, @CampaignId, @ProductType, @CampaignName)";

            using (var connection = await _connectionFactory.CreateOpenConnectionAsync())
            {
                var id = connection.QuerySingle<int>(sql, promocode);
                promocode.Id = id;
            }

            return promocode;
        }

        public async Task<Promocode> Update(Promocode promocode)
        {
            var sql =
                $"UPDATE dbo.Promocodes " +
                $"SET [ValidityStartDate]=@ValidityStartDate, [ValidityEndDate]=@ValidityEndDate, [RedemptionQuantity]=@RedemptionQuantity, [RedeemedQuantity]=@RedeemedQuantity, [DiscountType]=@DiscountType, [DiscountAmount]=@DiscountAmount, [CurrencyCode]=@CurrencyCode, [ValidationRuleId]=@ValidationRuleId, [PromocodeId]=@PromocodeId, [CampaignId]=@CampaignId, [ProductType]=@ProductType, [CampaignName]=@CampaignName, [RetentionDate] = @RetentionDate " +
                $"WHERE [Code]=@Code";
            using (var connection = await _connectionFactory.CreateOpenConnectionAsync())
            {
                await connection.ExecuteAsync(sql, promocode);
            }

            return promocode;
        }
    }
}
