using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentMigrator;

namespace Trainline.PromocodeService.Host.Migrations
{
    [Migration(13, "Add Product Type Column to Promocodes Table")]
    public class M013_AddProductTypeColumnToPromocodesTableMigration : Migration
    {
        private const string PromocodesTable = "Promocodes";
        private const string ProductType = "ProductType";
        public override void Up()
        {
            Create.Column(ProductType).OnTable(PromocodesTable).AsCustom("varchar(36)").WithDefaultValue("travel");
        }

        public override void Down()
        {
            Delete.Column(ProductType).FromTable(PromocodesTable);
        }
    }
}
