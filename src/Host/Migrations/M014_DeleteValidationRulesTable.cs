using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentMigrator;

namespace Trainline.PromocodeService.Host.Migrations
{
    [Migration(14, "DeleteValidation Rules Table")]
    public class M014_DeleteValidationRulesTableMigration : Migration
    {
        private const string TableName = "ValidationRules";
        public override void Up()
        {
            Delete.Table(TableName);
        }

        public override void Down()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt64().Identity().PrimaryKey($"PK_{TableName}_Id")
                .WithColumn("RuleId").AsCustom("varchar(100)").NotNullable()
                .WithColumn("Name").AsCustom("varchar(100)").NotNullable()
                .WithColumn("Value").AsCustom("varchar(50)").NotNullable();



            Create.Index($"IX_{TableName}_RuleId")
                .OnTable(TableName)
                .OnColumn("RuleId").Ascending()
                .WithOptions()
                .NonClustered();
        }
    }
}
