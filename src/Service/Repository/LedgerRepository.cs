using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trainline.PromocodeService.Service.Repository.Entities;

namespace Trainline.PromocodeService.Service.Repository
{

    public class LedgerRepository : ILedgerRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;


        const string productInfoSql =
            "INSERT INTO dbo.LedgerProductsInfo ([LedgerId], [ProductUri], [ProductPrice], [RootProductUri], [LinkId]) " +
            "OUTPUT INSERTED.Id " +
            "VALUES (@LedgerId, @ProductUri, @ProductPrice, @RootProductUri, @LinkId)";

        public LedgerRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Ledger> Add(Ledger ledger)
        {
            const string ledgerSql =
                "INSERT INTO dbo.Ledgers ([PromocodeId], [RedemptionId], [CurrencyCode], [PromoAmount]) " +
                "OUTPUT INSERTED.Id " +
                "VALUES (@PromocodeId, @RedemptionId, @CurrencyCode, @PromoAmount)";

            const string ledgerLineSql =
                "INSERT INTO dbo.LedgerLines ([LedgerId], [ProductUri], [Amount], [LinkId]) " +
                "OUTPUT INSERTED.Id " +
                "VALUES (@LedgerId, @ProductUri, @Amount, @LinkId)";
            
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var ledgerId = await transaction.Connection.QuerySingleAsync<int>(ledgerSql, ledger, transaction);
                ledger.Id = ledgerId;

                foreach (var line in ledger.Lines)
                {
                    line.LedgerId = ledgerId;
                    var lineId = await transaction.Connection.QuerySingleAsync<int>(ledgerLineSql, line, transaction);
                    line.Id = lineId;
                }

                foreach (var product in ledger.Products)
                {
                    product.LedgerId = ledgerId;
                    var productId = await transaction.Connection.QuerySingleAsync<int>(productInfoSql, product, transaction);
                    product.Id = productId;
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }

            return ledger;
        }

        public async Task<Ledger> Get(string promocodeId, string redemptionId)
        {
            const string ledgerSql =
                "SELECT [Id], [PromocodeId], [RedemptionId], [CurrencyCode], [PromoAmount] " +
                "FROM dbo.Ledgers " +
                "WHERE [PromocodeId]=@PromocodeId AND [RedemptionId]=@RedemptionId";

            const string ledgerLineSql =
                "SELECT [Id], [LedgerId], [ProductUri], [Amount], [LinkId] " +
                "FROM dbo.LedgerLines " +
                "WHERE [LedgerId]=@LedgerId";

            const string productInfoSql =
                "SELECT [Id], [LedgerId], [ProductUri], [ProductPrice], [RootProductUri], [LinkId] " +
                "FROM dbo.LedgerProductsInfo " +
                "WHERE [LedgerId]=@LedgerId";

            const string promoQuoteSql =
                "SELECT [Id], [LedgerId], [PromoQuoteId], [ReferenceId], [ProductUri], [DeductionAmount], [DeductionCurrencyCode], [Status], [Hash] " +
                "FROM dbo.LedgerQuotes " +
                "WHERE [LedgerId]=@LedgerId";

            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            var ledger = await connection.QuerySingleAsync<Ledger>(ledgerSql, new
            {
                PromocodeId = promocodeId,
                RedemptionId = redemptionId
            });

            var ledgerIdParam = new { LedgerId = ledger.Id };

            ledger.Lines = (await connection.QueryAsync<LedgerLine>(ledgerLineSql, ledgerIdParam)).ToList();
            ledger.Products = (await connection.QueryAsync<ProductInfo>(productInfoSql, ledgerIdParam)).ToList();
            ledger.Quotes = (await connection.QueryAsync<PromoQuote>(promoQuoteSql, ledgerIdParam)).ToList();

            return ledger;

        }

        public async Task<Ledger> Update(Ledger ledger)
        {
            const string ledgerLineSql =
                "INSERT INTO dbo.LedgerLines ([LedgerId], [ProductUri], [Amount], [LinkId]) " +
                "OUTPUT INSERTED.Id " +
                "VALUES (@LedgerId, @ProductUri, @Amount, @LinkId)";

            const string insertQuoteSql =
                "INSERT INTO dbo.LedgerQuotes ([LedgerId], [PromoQuoteId], [ReferenceId], [ProductUri], [DeductionAmount], [DeductionCurrencyCode], [Status], [Hash]) " +
                "OUTPUT INSERTED.Id " +
                "VALUES (@LedgerId, @PromoQuoteId, @ReferenceId, @ProductUri, @DeductionAmount, @DeductionCurrencyCode, @Status, @Hash)";

            const string updateQuoteSql =
                "UPDATE dbo.LedgerQuotes " +
                "SET [Status]=@Status " +
                "WHERE [Id]=@Id";

            using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var product in ledger.Products.Where(l => l.Id == 0))
                {
                    product.LedgerId = ledger.Id;
                    var productId = await transaction.Connection.QuerySingleAsync<int>(productInfoSql, product, transaction);
                    product.Id = productId;
                }

                foreach (var newLine in ledger.Lines.Where(l => l.Id == 0))
                {
                    newLine.LedgerId = ledger.Id;
                    var lineId = await transaction.Connection.QuerySingleAsync<int>(ledgerLineSql, newLine, transaction);
                    newLine.Id = lineId;
                }

                foreach (var oldQuote in ledger.Quotes.Where(l => l.Id != 0))
                {
                    await transaction.Connection.ExecuteAsync(updateQuoteSql, oldQuote, transaction);
                }

                foreach (var newQuote in ledger.Quotes.Where(l => l.Id == 0))
                {
                    newQuote.LedgerId = ledger.Id;
                    var quoteId = await transaction.Connection.QuerySingleAsync<int>(insertQuoteSql, newQuote, transaction);
                    newQuote.Id = quoteId;
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            return ledger;
        }

        public async Task RemoveLink(string promocodeId, string redemptionId, string linkId)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string ledgerSql =
                    "SELECT [Id], [PromocodeId], [RedemptionId], [CurrencyCode], [PromoAmount] " +
                    "FROM dbo.Ledgers " +
                    "WHERE [PromocodeId]=@PromocodeId AND [RedemptionId]=@RedemptionId";

                const string deleteLinesSql =
                    "DELETE " +
                    "FROM dbo.LedgerLines " +
                    "WHERE [LinkId]=@LinkId";

                const string deleteProductInfoSql =
                    "DELETE " +
                    "FROM dbo.LedgerProductsInfo " +
                    "WHERE [LinkId]=@LinkId";

                var ledger = await transaction.Connection.QuerySingleAsync<Ledger>(ledgerSql, new
                {
                    PromocodeId = promocodeId,
                    RedemptionId = redemptionId
                }, transaction);

                var queryParams = new
                {
                    LedgerId = ledger.Id,
                    LinkId = linkId
                };
                await transaction.Connection.ExecuteAsync(deleteLinesSql, queryParams, transaction);
                await transaction.Connection.ExecuteAsync(deleteProductInfoSql, queryParams, transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }        
        }
    }
}
