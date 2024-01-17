using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using Oqtane.Wiki.Models;

namespace Oqtane.Wiki.Services
{
    public class WikiService : ServiceBase, IWikiService, IService
    {
        private readonly SiteState _siteState;

        public WikiService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

         private string Apiurl => CreateApiUrl("Wiki", _siteState.Alias);

        public async Task<List<WikiContent>> GetWikiContentsAsync(int ModuleId, string Search, string Tag)
        {
            return await GetJsonAsync<List<WikiContent>>(CreateAuthorizationPolicyUrl($"{Apiurl}/search?moduleid={ModuleId}&&search={Search}&tag={Tag}", EntityNames.Module, ModuleId), Enumerable.Empty<WikiContent>().ToList());
        }

        public async Task<List<WikiContent>> GetWikiContentsAsync(int ModuleId, int WikiPageId)
        {
            return await GetJsonAsync<List<WikiContent>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}&wikipageid={WikiPageId}", EntityNames.Module, ModuleId), Enumerable.Empty<WikiContent>().ToList());
        }

        public async Task<WikiContent> GetWikiContentAsync(int ModuleId, int WikiPageId)
        {
            return await GetJsonAsync<WikiContent>(CreateAuthorizationPolicyUrl($"{Apiurl}/{WikiPageId}", EntityNames.Module, ModuleId));
        }

        public async Task<WikiContent> GetWikiContentAsync(int ModuleId, int WikiPageId, int WikiContentId)
        {
            return await GetJsonAsync<WikiContent>(CreateAuthorizationPolicyUrl($"{Apiurl}/{WikiPageId}/{WikiContentId}", EntityNames.Module, ModuleId));
        }

        public async Task<WikiContent> AddWikiContentAsync(WikiContent WikiContent)
        {
            return await PostJsonAsync<WikiContent>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, WikiContent.WikiPage.ModuleId), WikiContent);
        }

        public async Task DeleteWikiContentAsync(int WikiPageId, int WikiContentId)
        {
            await DeleteAsync($"{Apiurl}/{WikiPageId}/{WikiContentId}");
        }
        public async Task DeleteWikiPageAsync(int WikiPageId)
        {
            await DeleteAsync($"{Apiurl}/page/{WikiPageId}");
        }
    }
}
