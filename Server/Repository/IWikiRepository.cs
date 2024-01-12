using Oqtane.Wiki.Models;
using System.Collections.Generic;

namespace Oqtane.Wiki.Repository
{
    public interface IWikiRepository
    {
        IEnumerable<WikiContent> GetWikiContents(int ModuleId, string Search);
        IEnumerable<WikiContent> GetWikiContents(int ModuleId, int WikiPageId);
        WikiContent GetWikiContent(int WikiContentId);
        WikiPage GetWikiPage(int WikiPageId);
        WikiContent AddWikiContent(WikiContent WikiContent);
        void DeleteWikiContent(int WikiContentId);
        void DeleteWikiPage(int WikiPageId);
    }
}
