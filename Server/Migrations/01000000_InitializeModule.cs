using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Wiki.Migrations.EntityBuilders;
using Oqtane.Wiki.Repository;

namespace Oqtane.Wiki.Migrations
{
    [DbContext(typeof(WikiContext))]
    [Migration("Wiki.01.00.00.00")]
    public class InitializeModule : MultiDatabaseMigration
    {
        public InitializeModule(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var wikiPageEntityBuilder = new WikiPageEntityBuilder(migrationBuilder, ActiveDatabase);
            wikiPageEntityBuilder.Create();

            var wikiContentEntityBuilder = new WikiContentEntityBuilder(migrationBuilder, ActiveDatabase);
            wikiContentEntityBuilder.Create();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var wikiPageEntityBuilder = new WikiPageEntityBuilder(migrationBuilder, ActiveDatabase);
            wikiPageEntityBuilder.Drop();

            var wikiContentEntityBuilder = new WikiContentEntityBuilder(migrationBuilder, ActiveDatabase);
            wikiContentEntityBuilder.Drop();
        }
    }
}
