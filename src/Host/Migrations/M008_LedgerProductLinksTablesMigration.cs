using System;
using FluentMigrator;

namespace Trainline.PromocodeService.Host.Migrations
{

    [Migration(8, "Add fields to support product linking in ledger")]
    public class M008_LedgerProductLinksTablesMigration : Migration
    {
        private const string LedgerLinesTable = "LedgerLines";
        private const string LedgerProductsInfoTable = "LedgerProductsInfo";

        public override void Up()
        {
            Alter.Table(LedgerLinesTable).AddColumn("LinkId").AsCustom("varchar(36)").Nullable();
            Alter.Table(LedgerProductsInfoTable).AddColumn("RootProductUri").AsCustom("nvarchar(2048)").Nullable();
            Alter.Table(LedgerProductsInfoTable).AddColumn("LinkId").AsCustom("varchar(36)").Nullable();
        }

        public override void Down()
        {
            Delete.Column("LinkId").FromTable(LedgerLinesTable);
            Delete.Column("RootProductUri").FromTable(LedgerProductsInfoTable);
            Delete.Column("LinkId").FromTable(LedgerProductsInfoTable);
        }
    }
}
