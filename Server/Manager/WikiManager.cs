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

namespace Oqtane.Wiki.Manager
{
    public class WikiManager : MigratableModuleBase, IInstallable, IPortable, ISitemap
    {
        private IWikiRepository _WikiRepository;
		private readonly IDBContextDependencies _DBContextDependencies;

		public WikiManager(IWikiRepository WikiRepository, ISqlRepository sql, IDBContextDependencies DBContextDependencies)
        {
            _WikiRepository = WikiRepository;
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

        public string ExportModule(Module module)
        {
            string content = "";
            List<WikiContent> WikiContents = _WikiRepository.GetWikiContents(module.ModuleId, "").ToList();
            if (WikiContents != null)
            {
                content = JsonSerializer.Serialize(WikiContents);
            }
            return content;
        }

        public void ImportModule(Module module, string content, string version)
        {
            List<WikiContent> WikiContents = null;
            if (!string.IsNullOrEmpty(content))
            {
                WikiContents = JsonSerializer.Deserialize<List<WikiContent>>(content);
            }
            if (WikiContents != null)
            {
                foreach(WikiContent WikiContent in WikiContents)
                {
                    WikiContent wikiContent = new WikiContent();
                    wikiContent.ModuleId = module.ModuleId;
                    wikiContent.Title = WikiContent.Title;
                    wikiContent.Content = WikiContent.Content;
                    _WikiRepository.AddWikiContent(wikiContent);
                }
            }
        }

        public List<Sitemap> GetUrls(string alias, string path, Module module)
        {
            var sitemap = new List<Sitemap>();
            foreach (var wikiContent in _WikiRepository.GetWikiContents(module.ModuleId, ""))
            {
                var parameters = Utilities.AddUrlParameters(wikiContent.WikiPageId, Common.FormatSlug(wikiContent.Title));
                sitemap.Add(new Sitemap { Url = Utilities.NavigateUrl(alias, path, parameters), ModifiedOn = wikiContent.CreatedOn });
            }
            return sitemap;
        }
    }
}