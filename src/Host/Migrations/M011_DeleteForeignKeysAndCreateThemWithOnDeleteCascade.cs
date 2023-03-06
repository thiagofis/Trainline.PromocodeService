using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentMigrator;

namespace Trainline.PromocodeService.Host.Migrations
{
    [Migration(11, "Delete Foreign Keys and Add them with On Delete Cascade Rule")]
    public class M011_ModifyForeignKeysToHaveOnDeleteCascade : Migration
    {
        public override void Up()
        {
            Delete.ForeignKey("FK_LedgerLines_Ledgers_Id").OnTable("LedgerLines");
            Delete.ForeignKey("FK_LedgerProductsInfo_Ledgers_Id").OnTable("LedgerProductsInfo");
            Delete.ForeignKey("FK_LedgerQuotes_Ledgers_Id").OnTable("LedgerQuotes");
            Delete.ForeignKey("FK_Redemptions_RedemptionId").OnTable("Ledgers");
            Delete.ForeignKey("FK_Redemptions_PromocodeId").OnTable("Redemptions");

            Create.ForeignKey("FK_LedgerLines_Ledgers_Id").FromTable("LedgerLines").ForeignColumn("LedgerId")
                .ToTable("Ledgers").PrimaryColumn("Id").OnDelete(Rule.Cascade);
            Create.ForeignKey("FK_LedgerProductsInfo_Ledgers_Id").FromTable("LedgerProductsInfo").ForeignColumn("LedgerId")
                .ToTable("Ledgers").PrimaryColumn("Id").OnDelete(Rule.Cascade);
            Create.ForeignKey("FK_LedgerQuotes_Ledgers_Id").FromTable("LedgerQuotes").ForeignColumn("LedgerId")
                .ToTable("Ledgers").PrimaryColumn("Id").OnDelete(Rule.Cascade);

            Create.ForeignKey("FK_Redemptions_RedemptionId").FromTable("Ledgers").ForeignColumn("RedemptionId")
                .ToTable("Redemptions").PrimaryColumn("RedemptionId").OnDelete(Rule.Cascade);

            Create.ForeignKey("FK_Redemptions_PromocodeId").FromTable("Redemptions").ForeignColumn("PromocodeId")
                .ToTable("Promocodes").PrimaryColumn("PromocodeId").OnDelete(Rule.Cascade);


        }

        public override void Down()
        {
            Delete.ForeignKey("FK_LedgerLines_Ledgers_Id").OnTable("LedgerLines");
            Delete.ForeignKey("FK_LedgerProductsInfo_Ledgers_Id").OnTable("LedgerProductsInfo");
            Delete.ForeignKey("FK_LedgerQuotes_Ledgers_Id").OnTable("LedgerQuotes");
            Delete.ForeignKey("FK_Redemptions_RedemptionId").OnTable("Ledgers");
            Delete.ForeignKey("FK_Redemptions_PromocodeId").OnTable("Redemptions");

            Create.ForeignKey("FK_LedgerLines_Ledgers_Id").FromTable("LedgerLines").ForeignColumn("LedgerId")
                .ToTable("Ledgers").PrimaryColumn("Id");
            Create.ForeignKey("FK_LedgerProductsInfo_Ledgers_Id").FromTable("LedgerProductsInfo").ForeignColumn("LedgerId")
                .ToTable("Ledgers").PrimaryColumn("Id");
            Create.ForeignKey("FK_LedgerQuotes_Ledgers_Id").FromTable("LedgerQuotes").ForeignColumn("LedgerId")
                .ToTable("Ledgers").PrimaryColumn("Id");

            Create.ForeignKey("FK_Redemptions_RedemptionId").FromTable("Ledgers").ForeignColumn("RedemptionId")
                .ToTable("Redemptions").PrimaryColumn("RedemptionId");

            Create.ForeignKey("FK_Redemptions_PromocodeId").FromTable("Redemptions").ForeignColumn("PromocodeId")
                .ToTable("Promocodes").PrimaryColumn("PromocodeId");
        }
    }
}
