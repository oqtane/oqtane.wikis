using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Repository.Databases.Interfaces;
using Oqtane.Wiki.Models;

namespace Oqtane.Wiki.Repository
{
    public class WikiContext : DBContextBase, IService, IMultiDatabase
    {
        public virtual DbSet<WikiPage> WikiPage { get; set; }
        public virtual DbSet<WikiContent> WikiContent { get; set; }
        public virtual DbSet<WikiLink> WikiLink { get; set; }

        public WikiContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
        {
            // ContextBase handles multi-tenant database connections
        }
    }
}
