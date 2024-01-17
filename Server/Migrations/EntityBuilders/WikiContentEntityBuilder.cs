using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.Wiki.Migrations.EntityBuilders
{
    public class WikiContentEntityBuilder : BaseEntityBuilder<WikiContentEntityBuilder>
    {
        private const string _entityTableName = "WikiContent";
        private readonly PrimaryKey<WikiContentEntityBuilder> _primaryKey = new("PK_WikiContent", x => x.WikiContentId);
        private readonly ForeignKey<WikiContentEntityBuilder> _moduleForeignKey = new("FK_WikiContent_WikiPage", x => x.WikiPageId, "WikiPage", "WikiPageId", ReferentialAction.Cascade);

        public WikiContentEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override WikiContentEntityBuilder BuildTable(ColumnsBuilder table)
        {
            WikiContentId = AddAutoIncrementColumn(table, "WikiContentId");
            WikiPageId = AddIntegerColumn(table, "WikiPageId");
            Content = AddMaxStringColumn(table, "Content");
            CreatedBy = AddStringColumn(table, "CreatedBy", 256);
            CreatedOn = AddDateTimeColumn(table, "CreatedOn");

            return this;
        }

        public OperationBuilder<AddColumnOperation> WikiContentId { get; set; }
        public OperationBuilder<AddColumnOperation> WikiPageId { get; set; }
        public OperationBuilder<AddColumnOperation> Content { get; set; }
        public OperationBuilder<AddColumnOperation> CreatedBy { get; set; }
        public OperationBuilder<AddColumnOperation> CreatedOn { get; set; }
    }
}
