using FluentMigrator;

namespace Trainline.PromocodeService.Host.Migrations
{
    [Migration(3, "Add new Redeemed table")]
    public class M003_CreateRedemtionTableMigration : Migration
    {
        private const string TableName = "Redemptions";
        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt64().Identity().PrimaryKey($"PK_{TableName}_Id")
                .WithColumn("Code")
                    .AsCustom("varchar(36)")
                    .NotNullable()
                    .ForeignKey($"FK_{TableName}_Code", "Promocodes", "Code")
                .WithColumn("RedemptionId").AsCustom("varchar(36)").NotNullable()
                .Unique($"UQ_{TableName}_RedemptionId");
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}
