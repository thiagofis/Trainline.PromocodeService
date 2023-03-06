using FluentMigrator;

namespace Trainline.PromocodeService.Host.Migrations
{
    [Migration(4, "Add new ValidationRules column")]
    public class M004_CreateValidationRuleIdColumnMigration : Migration
    {
        private const string TableName = "Promocodes";
        private const string ColumnName = "ValidationRuleId";
        public override void Up()
        {
            Alter.Table(TableName)
                .AddColumn(ColumnName)
                .AsCustom("varchar(100)")
                .Nullable();
        }

        public override void Down()
        {
            Delete.Column(ColumnName).FromTable(TableName);
        }
    }
}
