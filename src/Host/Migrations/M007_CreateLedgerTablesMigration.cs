using System;
using FluentMigrator;

namespace Trainline.PromocodeService.Host.Migrations
{

    [Migration(7, "Add new Ledgers, LedgerLines, LedgerProductsInfo, LedgerQuotes tables")]
    public class M007_CreateLedgerTablesMigration : Migration
    {
        private const string LedgersTable = "Ledgers";
        private const string LedgerLinesTable = "LedgerLines";
        private const string LedgerProductsInfoTable = "LedgerProductsInfo";
        private const string LedgerQuotesTable = "LedgerQuotes";
        private const string PromocodesTable = "Promocodes";
        private const string RedemptionsTable = "Redemptions";

        public override void Up()
        {
            Create.Table(LedgersTable)
                .WithColumn("Id").AsInt64().Identity().PrimaryKey($"PK_{LedgersTable}_Id")
                .WithColumn("PromocodeId").AsCustom("varchar(36)").NotNullable()
                    .ForeignKey($"FK_{PromocodesTable}_PromocodeId", PromocodesTable, "PromocodeId")
                .WithColumn("RedemptionId").AsCustom("varchar(36)").NotNullable()
                    .ForeignKey($"FK_{RedemptionsTable}_RedemptionId", RedemptionsTable, "RedemptionId")
                .WithColumn("CurrencyCode").AsCustom("varchar(3)").NotNullable()
                .WithColumn("PromoAmount").AsDecimal(10, 2).NotNullable();

            Create.Index().OnTable(LedgersTable)
                .OnColumn("PromocodeId").Descending()
                .OnColumn("RedemptionId").Descending();
                
            Create.Table(LedgerLinesTable)
                .WithColumn("Id").AsInt64().Identity().PrimaryKey($"PK_{LedgerLinesTable}_Id")
                .WithColumn("LedgerId").AsInt64().NotNullable()
                    .ForeignKey($"FK_{LedgerLinesTable}_{LedgersTable}_Id", LedgersTable, "Id")
                .WithColumn("ProductUri").AsCustom("nvarchar(2048)").NotNullable()
                .WithColumn("Amount").AsDecimal(10, 2).NotNullable();

            Create.Index().OnTable(LedgerLinesTable)
                .OnColumn("LedgerId").Descending();

            Create.Table(LedgerProductsInfoTable)
                .WithColumn("Id").AsInt64().Identity().PrimaryKey($"PK_{LedgerProductsInfoTable}_Id")
                .WithColumn("LedgerId").AsInt64().NotNullable()
                    .ForeignKey($"FK_{LedgerProductsInfoTable}_{LedgersTable}_Id", LedgersTable, "Id")
                .WithColumn("ProductUri").AsCustom("nvarchar(2048)").NotNullable()
                .WithColumn("ProductPrice").AsDecimal(10, 2).NotNullable();

            Create.Index().OnTable(LedgerProductsInfoTable)
                .OnColumn("LedgerId").Descending();

            Create.Table(LedgerQuotesTable)
                .WithColumn("Id").AsInt64().Identity().PrimaryKey($"PK_{LedgerQuotesTable}_Id")
                .WithColumn("LedgerId").AsInt64().NotNullable()
                    .ForeignKey($"FK_{LedgerQuotesTable}_{LedgersTable}_Id", LedgersTable, "Id")
                .WithColumn("PromoQuoteId").AsCustom("varchar(36)").NotNullable()
                .WithColumn("ReferenceId").AsCustom("nvarchar(2048)").NotNullable()
                .WithColumn("ProductUri").AsCustom("nvarchar(2048)").NotNullable()
                .WithColumn("DeductionAmount").AsDecimal(10, 2).NotNullable()
                .WithColumn("DeductionCurrencyCode").AsCustom("varchar(3)").NotNullable()
                .WithColumn("Status").AsInt16().NotNullable()
                .WithColumn("Hash").AsCustom("varchar(32)").NotNullable();

            Create.Index().OnTable(LedgerQuotesTable)
                .OnColumn("LedgerId").Descending();
        }

        public override void Down()
        {
            Delete.Table(LedgerQuotesTable);
            Delete.Table(LedgerLinesTable);
            Delete.Table(LedgerProductsInfoTable);
            Delete.Table(LedgersTable);
        }
    }
}
