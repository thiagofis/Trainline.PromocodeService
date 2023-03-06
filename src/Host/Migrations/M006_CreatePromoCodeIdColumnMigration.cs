using System;
using FluentMigrator;

namespace Trainline.PromocodeService.Host.Migrations
{
    [Migration(6, "Add new PromocodeId column to Promocodes table and replace Code for it in Redemptions table")]
    public class M006_CreatePromoCodeIdColumnMigration : Migration
    {
        private const string PromocodesTable = "Promocodes";
        private const string RedemptionsTable = "Redemptions";
        private const string ColumnName = "PromocodeId";

        public override void Up()
        {
            Delete.Table(RedemptionsTable);
            Delete.Table(PromocodesTable);

            Create.Table(PromocodesTable)
                .WithColumn("Id").AsInt64().Identity().PrimaryKey($"PK_{PromocodesTable}_Id")
                .WithColumn("PromocodeId").AsCustom("varchar(36)").NotNullable().Unique($"UQ_{PromocodesTable}_{ColumnName}")
                .WithColumn("Code").AsCustom("varchar(36)").NotNullable().Unique($"UQ_{PromocodesTable}_Code")
                .WithColumn("ValidityStartDate").AsDateTime().NotNullable()
                .WithColumn("ValidityEndDate").AsDateTime().NotNullable()
                .WithColumn("RedemptionQuantity").AsInt32().Nullable()
                .WithColumn("RedeemedQuantity").AsInt32().NotNullable()
                .WithColumn("DiscountType").AsCustom("varchar(36)").NotNullable()
                .WithColumn("DiscountAmount").AsDecimal(10, 2).NotNullable()
                .WithColumn("CurrencyCode").AsCustom("varchar(3)").NotNullable()
                .WithColumn("ValidationRuleId").AsCustom("varchar(100)").Nullable();

            Create.Table(RedemptionsTable)
                .WithColumn("Id").AsInt64().Identity().PrimaryKey($"PK_{RedemptionsTable}_Id")
                .WithColumn("PromocodeId")
                .AsCustom("varchar(36)")
                .NotNullable()
                .ForeignKey($"FK_{RedemptionsTable}_PromocodeId", "Promocodes", "PromocodeId")
                .WithColumn("RedemptionId").AsCustom("varchar(36)").NotNullable()
                .Unique($"UQ_{RedemptionsTable}_RedemptionId");

        }

        public override void Down()
        {
            Delete.Table(RedemptionsTable);
            Delete.Table(PromocodesTable);

            Create.Table(PromocodesTable)
                .WithColumn("Id").AsInt64().Identity().PrimaryKey($"PK_{PromocodesTable}_Id")
                .WithColumn("Code").AsCustom("varchar(36)").NotNullable().Unique($"UQ_{PromocodesTable}_Code")
                .WithColumn("ValidityStartDate").AsDateTime().NotNullable()
                .WithColumn("ValidityEndDate").AsDateTime().NotNullable()
                .WithColumn("RedemptionQuantity").AsInt32().Nullable()
                .WithColumn("RedeemedQuantity").AsInt32().NotNullable()
                .WithColumn("DiscountType").AsCustom("varchar(36)").NotNullable()
                .WithColumn("DiscountAmount").AsDecimal(10, 2).NotNullable()
                .WithColumn("CurrencyCode").AsCustom("varchar(3)").NotNullable()
                .WithColumn("ValidationRuleId").AsCustom("varchar(100)").Nullable();

            Create.Table(RedemptionsTable)
                .WithColumn("Id").AsInt64().Identity().PrimaryKey($"PK_{RedemptionsTable}_Id")
                .WithColumn("Code")
                .AsCustom("varchar(36)")
                .NotNullable()
                .ForeignKey($"FK_{RedemptionsTable}_Code", "Promocodes", "Code")
                .WithColumn("RedemptionId").AsCustom("varchar(36)").NotNullable()
                .Unique($"UQ_{RedemptionsTable}_RedemptionId");
        }
    }
}
