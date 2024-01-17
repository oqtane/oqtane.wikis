using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Oqtane.Modules;
using Oqtane.Models;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using Oqtane.Wiki.Repository;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Wiki.Shared;
using Oqtane.Wiki.Models;
using System;

namespace Oqtane.Wiki.Manager
{
    public class WikiManager : MigratableModuleBase, IInstallable, ISitemap
    {
        private IWikiRepository _WikiRepository;
        private ISettingRepository _SettingRepository;
        private readonly IDBContextDependencies _DBContextDependencies;

		public WikiManager(IWikiRepository WikiRepository, ISettingRepository SettingRepository, IDBContextDependencies DBContextDependencies)
        {
            _WikiRepository = WikiRepository;
            _SettingRepository = SettingRepository;
            _DBContextDependencies = DBContextDependencies;
        }

        public bool Install(Tenant tenant, string version)
        {
            return Migrate(new WikiContext(_DBContextDependencies), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new WikiContext(_DBContextDependencies), tenant, MigrationType.Down);
        }

        public List<Sitemap> GetUrls(string alias, string path, Module module)
        {
            var sitemap = new List<Sitemap>();
            foreach (var wikiContent in _WikiRepository.GetWikiContents(module.ModuleId, "", ""))
            {
                var parameters = Utilities.AddUrlParameters(wikiContent.WikiPageId, Common.FormatSlug(wikiContent.WikiPage.Title));
                sitemap.Add(new Sitemap { Url = Utilities.NavigateUrl(alias, path, parameters), ModifiedOn = wikiContent.CreatedOn });
            }
            return sitemap;
        }
    }
}