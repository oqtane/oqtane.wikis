using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Microsoft.EntityFrameworkCore;
using Oqtane.Wiki.Models;
using Oqtane.Shared;
using Oqtane.Wiki.Shared;

namespace Oqtane.Wiki.Repository
{
    public class WikiRepository : IWikiRepository, IService
    {
        private readonly WikiContext _db;

        public WikiRepository(WikiContext context)
        {
            _db = context;
        }

        public IEnumerable<WikiContent> GetWikiContents(int ModuleId, string Search)
        {
            return _db.WikiContent.Include(item => item.WikiPage)
                .Where(item => item.WikiPage.ModuleId == ModuleId)
                .GroupBy(item => item.WikiPageId)
                .Select(item => item.OrderBy(item => item.CreatedOn).Last()).ToList()
                .Where(item => string.IsNullOrEmpty(Search) || item.Title.ToLower().Contains(Search.ToLower()) || item.Content.ToLower().Contains(Search.ToLower()));
        }

        public IEnumerable<WikiContent> GetWikiContents(int ModuleId, int WikiPageId)
        {
            return _db.WikiContent.Include(item => item.WikiPage)
                .Where(item => item.WikiPage.ModuleId == ModuleId && item.WikiPageId == WikiPageId);
        }

        public WikiContent GetWikiContent(int WikiContentId)
        {
            return _db.WikiContent.Find(WikiContentId);
        }

        public WikiPage GetWikiPage(int WikiPageId)
        {
            return _db.WikiPage.Find(WikiPageId);
        }

        public WikiContent AddWikiContent(WikiContent WikiContent)
        {
            WikiContent.Content = ManageWikiLinks(WikiContent);

            if (WikiContent.WikiPageId == -1)
            {
                var wikipage = new WikiPage();
                wikipage.ModuleId = WikiContent.ModuleId;
                wikipage.CreatedBy = WikiContent.CreatedBy;
                wikipage.CreatedOn = WikiContent.CreatedOn;
                _db.WikiPage.Add(wikipage);
                _db.SaveChanges();
                WikiContent.WikiPageId = wikipage.WikiPageId;
            }

            _db.WikiContent.Add(WikiContent);
            _db.SaveChanges();

            return WikiContent;
        }

        private string ManageWikiLinks(WikiContent WikiContent)
        {
            var content = WikiContent.Content;

            // wiki links are in the form [[link]]
            var links = new List<string>();
            int index = content.IndexOf("[[");
            while (index != -1)
            {
                if (content.IndexOf("]]", index) != -1)
                {
                    links.Add(content.Substring(index + 2, content.IndexOf("]]", index) - index - 2));
                }
                index = content.IndexOf("[[", index + 1);
            }

            if (links.Count > 0)
            {
                var wikicontents = GetWikiContents(WikiContent.ModuleId, "").ToList();

                foreach (var link in links)
                {
                    var wikicontent = wikicontents.FirstOrDefault(item => item.Title == link);

                    if (wikicontent == null)
                    {
                        // create new WikiPage
                        var wikipage = new WikiPage();
                        wikipage.ModuleId = WikiContent.ModuleId;
                        wikipage.CreatedBy = WikiContent.CreatedBy;
                        wikipage.CreatedOn = WikiContent.CreatedOn;
                        _db.WikiPage.Add(wikipage);
                        _db.SaveChanges();

                        // create initial WikiContent
                        wikicontent = new WikiContent();
                        wikicontent.WikiPageId = wikipage.WikiPageId;
                        wikicontent.Title = link;
                        wikicontent.Content = $"<p>{wikicontent.Title}</p>";
                        wikicontent.CreatedBy = WikiContent.CreatedBy;
                        wikicontent.CreatedOn = WikiContent.CreatedOn;
                        _db.WikiContent.Add(wikicontent);
                        _db.SaveChanges();
                    }

                    // replace wiki link with hyperlink
                    var parameters = Utilities.AddUrlParameters(wikicontent.WikiPageId, Common.FormatSlug(wikicontent.Title));
                    content = WikiContent.Content.Replace($"[[{link}]]", $"<a href=\"{Utilities.NavigateUrl(WikiContent.AliasPath, WikiContent.PagePath, parameters)}\">{link}</a>");
                }
            }

            return content;
        }

        public void DeleteWikiContent(int WikiContentId)
        {
            WikiContent WikiContent = _db.WikiContent.Find(WikiContentId);
            _db.WikiContent.Remove(WikiContent);
            _db.SaveChanges();
        }

        public void DeleteWikiPage(int WikiPageId)
        {
            WikiPage WikiPage = _db.WikiPage.Find(WikiPageId);
            _db.WikiPage.Remove(WikiPage);
            _db.SaveChanges();
        }
    }
}
