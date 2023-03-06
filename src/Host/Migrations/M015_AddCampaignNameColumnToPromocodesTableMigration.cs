using FluentMigrator;

namespace Trainline.PromocodeService.Host.Migrations
{
    [Migration(15, "Add Campaign Name Column to Promocodes Table")]
    public class M015_AddCampaignNameToPromocodesTableMigration : Migration
    {
        private const string PromocodesTable = "Promocodes";
        private const string CampaignName = "CampaignName";
        public override void Up()
        {
            Create.Column(CampaignName).OnTable(PromocodesTable).AsCustom("varchar(36)").Nullable();
        }

        public override void Down()
        {
            Delete.Column(CampaignName).FromTable(PromocodesTable);
        }
    }
}
