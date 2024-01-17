using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.Wiki.Migrations.EntityBuilders
{
    public class WikiPageEntityBuilder : BaseEntityBuilder<WikiPageEntityBuilder>
    {
        private const string _entityTableName = "WikiPage";
        private readonly PrimaryKey<WikiPageEntityBuilder> _primaryKey = new("PK_WikiPage", x => x.WikiPageId);
        private readonly ForeignKey<WikiPageEntityBuilder> _moduleForeignKey = new("FK_WikiPage_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public WikiPageEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override WikiPageEntityBuilder BuildTable(ColumnsBuilder table)
        {
            WikiPageId = AddAutoIncrementColumn(table, "WikiPageId");
            ModuleId = AddIntegerColumn(table, "ModuleId");
            Title = AddStringColumn(table, "Title", 256, true, true);
            Tags = AddStringColumn(table, "Tags", 256, true, true);
            CreatedBy = AddStringColumn(table, "CreatedBy", 256);
            CreatedOn = AddDateTimeColumn(table, "CreatedOn");
            ModifiedBy = AddStringColumn(table, "ModifiedBy", 256);
            ModifiedOn = AddDateTimeColumn(table, "ModifiedOn");

            return this;
        }

        public OperationBuilder<AddColumnOperation> WikiPageId { get; set; }
        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
        public OperationBuilder<AddColumnOperation> Title { get; set; }
        public OperationBuilder<AddColumnOperation> Tags { get; set; }
        public OperationBuilder<AddColumnOperation> CreatedBy { get; set; }
        public OperationBuilder<AddColumnOperation> CreatedOn { get; set; }
        public OperationBuilder<AddColumnOperation> ModifiedBy { get; set; }
        public OperationBuilder<AddColumnOperation> ModifiedOn { get; set; }
    }
}
