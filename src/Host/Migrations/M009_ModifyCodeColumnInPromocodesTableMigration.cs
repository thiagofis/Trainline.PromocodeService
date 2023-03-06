using FluentMigrator;

namespace Trainline.PromocodeService.Host.Migrations
{
    [Migration(9, "Modify Code Column Size in Promocodes Table.")]
    public class M009_ModifyCodeColumnInPromocodesTableMigration : Migration
    {
        private const string PromocodesTable = "Promocodes";

        public override void Up()
        {
            Alter.Column("Code").OnTable(PromocodesTable).AsCustom("varchar(128)");
        }

        public override void Down()
        {
            Alter.Column("Code").OnTable(PromocodesTable).AsCustom("varchar(36)");
        }
    }
}
