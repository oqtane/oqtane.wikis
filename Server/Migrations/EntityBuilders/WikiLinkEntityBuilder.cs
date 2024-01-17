using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.Wiki.Migrations.EntityBuilders
{
    public class WikiLinkEntityBuilder : BaseEntityBuilder<WikiLinkEntityBuilder>
    {
        private const string _entityTableName = "WikiLink";
        private readonly PrimaryKey<WikiLinkEntityBuilder> _primaryKey = new("PK_WikiLink", x => x.WikiLinkId);
        private readonly ForeignKey<WikiLinkEntityBuilder> _moduleForeignKey = new("FK_WikiLink_WikiPage", x => x.FromWikiPageId, "WikiPage", "WikiPageId", ReferentialAction.Cascade);

        public WikiLinkEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override WikiLinkEntityBuilder BuildTable(ColumnsBuilder table)
        {
            WikiLinkId = AddAutoIncrementColumn(table, "WikiLinkId");
            FromWikiPageId = AddIntegerColumn(table, "FromWikiPageId");
            ToWikiPageId = AddIntegerColumn(table, "ToWikiPageId");
            CreatedBy = AddStringColumn(table, "CreatedBy", 256);
            CreatedOn = AddDateTimeColumn(table, "CreatedOn");

            return this;
        }

        public OperationBuilder<AddColumnOperation> WikiLinkId { get; set; }
        public OperationBuilder<AddColumnOperation> FromWikiPageId { get; set; }
        public OperationBuilder<AddColumnOperation> ToWikiPageId { get; set; }
        public OperationBuilder<AddColumnOperation> CreatedBy { get; set; }
        public OperationBuilder<AddColumnOperation> CreatedOn { get; set; }
    }
}
