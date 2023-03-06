using FluentMigrator;

namespace Trainline.PromocodeService.Host.Migrations
{
    [Migration(5, "Create Validation Rules Table")]
    public class M005CreateValidationRulesTableMigration : Migration
    {
        private const string TableName = "ValidationRules";
        public override void Up()
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

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}
