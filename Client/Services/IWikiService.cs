using Oqtane.Wiki.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Oqtane.Wiki.Services
{
    public interface IWikiService 
    {
        Task<List<WikiContent>> GetWikiContentsAsync(int ModuleId, string Search);
        Task<List<WikiContent>> GetWikiContentsAsync(int ModuleId, int WikiPageId);
        Task<WikiContent> GetWikiContentAsync(int ModuleId, int WikiPageId);
        Task<WikiContent> GetWikiContentAsync(int ModuleId, int WikiPageId, int WikiContentId);
        Task<WikiContent> AddWikiContentAsync(WikiContent WikiContent);
        Task DeleteWikiContentAsync(int WikiContentId);
        Task DeleteWikiPageAsync(int WikiPageId);
    }
}
