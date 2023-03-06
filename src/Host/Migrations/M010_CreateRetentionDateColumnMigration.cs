using System;
using FluentMigrator;

namespace Trainline.PromocodeService.Host.Migrations
{
    [Migration(10, "Add RetentionDate column to Promocodes Table")]
    public class M010_CreateRetentionDateColumnMigration : Migration
    {
        private const string PromocodesTable = "Promocodes";

        public override void Up()
        {
            Create.Column("RetentionDate").OnTable(PromocodesTable).AsDateTime().SetExistingRowsTo(DateTime.Now.AddMonths(43)).NotNullable();
        }

        public override void Down()
        {
            Delete.Column("RetentionDate");
        }
    }
}
