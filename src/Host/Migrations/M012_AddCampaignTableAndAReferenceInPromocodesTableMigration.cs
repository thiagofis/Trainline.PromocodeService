using System.Data;
using FluentMigrator;

namespace Trainline.PromocodeService.Host.Migrations
{
    [Migration(12, "Create Campaign Table and add a reference to it on Promocodes Table")]
    public class M012_AddCampaignTableAndAReferenceInPromocodesTableMigration : Migration
    {
        private const string CampaignsTable = "Campaigns";
        private const string PromocodesTable = "Promocodes";
        public override void Up()
        {
            Create.Table(CampaignsTable)
                .WithColumn("Id").AsInt64().Identity().PrimaryKey($"_{CampaignsTable}_Id")
                .WithColumn("CampaignId").AsCustom("varchar(36)").NotNullable().Unique($"UQ_{CampaignsTable}_CampaignId")
                .WithColumn("Redeemable").AsBoolean().WithDefaultValue(true);

            Create.Column("CampaignId").OnTable(PromocodesTable).AsCustom("varchar(36)").Nullable()
                .ForeignKey("FK_Campaigns_Id", CampaignsTable, "CampaignId").OnDelete(Rule.Cascade);
        }

        public override void Down()
        {
            Delete.Table(CampaignsTable);

            Delete.Column("CampaignId").FromTable(PromocodesTable);
        }
    }
}
