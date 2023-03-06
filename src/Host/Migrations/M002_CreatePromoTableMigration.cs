using FluentMigrator;

namespace Trainline.PromocodeService.Host.Migrations
{
    [Migration(2, "Add new Promocode table")]
    public class M002_CreatePromoTableMigration : Migration
    {
        private const string TableName = "Promocodes";
        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt64().Identity().PrimaryKey($"PK_{TableName}_Id")
                .WithColumn("Code").AsCustom("varchar(36)").NotNullable().Unique($"UQ_{TableName}_Code")
                .WithColumn("ValidityStartDate").AsDateTime().NotNullable()
                .WithColumn("ValidityEndDate").AsDateTime().NotNullable()
                .WithColumn("RedemptionQuantity").AsInt32().Nullable()
                .WithColumn("RedeemedQuantity").AsInt32().NotNullable()
                .WithColumn("DiscountType").AsCustom("varchar(36)").NotNullable()
                .WithColumn("DiscountAmount").AsDecimal(10, 2).NotNullable()
                .WithColumn("CurrencyCode").AsCustom("varchar(3)").NotNullable();
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}
