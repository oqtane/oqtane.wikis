using Oqtane.Wiki.Models;
using System.Collections.Generic;

namespace Oqtane.Wiki.Repository
{
    public interface IWikiRepository
    {
        IEnumerable<WikiContent> GetWikiContents(int ModuleId, string Search, string Tag);
        IEnumerable<WikiContent> GetWikiContents(int ModuleId, int WikiPageId);
        WikiContent GetWikiContent(int WikiPageId, int WikiContentId);
        WikiContent AddWikiContent(WikiContent WikiContent);
        void DeleteWikiContent(int WikiContentId);

        WikiPage GetWikiPage(int WikiPageId);
        void DeleteWikiPage(int WikiPageId);

        IEnumerable<WikiLink> GetWikiLinks(int ToWikiPageId);
    }
}
