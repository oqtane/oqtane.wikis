using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Microsoft.EntityFrameworkCore;
using Oqtane.Wiki.Models;
using Oqtane.Shared;
using Oqtane.Wiki.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;

namespace Oqtane.Wiki.Repository
{
    public class WikiRepository : IWikiRepository, IService
    {
        private readonly WikiContext _db;

        public WikiRepository(WikiContext context)
        {
            _db = context;
        }

        public IEnumerable<WikiContent> GetWikiContents(int ModuleId, string Search, string Tag)
        {
            var wikicontents = _db.WikiContent.Include(item => item.WikiPage)
                .Where(item => item.WikiPage.ModuleId == ModuleId)
                .GroupBy(item => item.WikiPageId)
                .Select(item => item.OrderBy(item => item.CreatedOn).Last()).ToList();

            if (!string.IsNullOrEmpty(Search))
            {
                wikicontents = wikicontents.Where(item => item.WikiPage.Title.ToLower().Contains(Search.ToLower()) || item.Content.ToLower().Contains(Search.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(Tag))
            {
                wikicontents = wikicontents.Where(item => item.WikiPage.Tags.ToLower().Split(',').Select(t => t.Trim()).Contains(Tag.ToLower())).ToList();
            }

            return wikicontents;
        }

        public IEnumerable<WikiContent> GetWikiContents(int ModuleId, int WikiPageId)
        {
            return _db.WikiContent.Include(item => item.WikiPage)
                .Where(item => item.WikiPage.ModuleId == ModuleId && item.WikiPageId == WikiPageId);
        }

        public WikiContent GetWikiContent(int WikiPageId, int WikiContentId)
        {
            var wikicontents = _db.WikiContent.Include(item => item.WikiPage)
                .Where(item => item.WikiPageId == WikiPageId);

            if (wikicontents?.Any() == true)
            {
                if (WikiContentId == -1)
                {
                    return wikicontents.OrderBy(item => item.CreatedOn).Last();
                }
                else
                {
                    return wikicontents.FirstOrDefault(item => item.WikiContentId == WikiContentId);
                }
            }
            return null;
        }

        public WikiContent AddWikiContent(WikiContent WikiContent)
        {
            if (WikiContent.WikiPageId == -1)
            {
                // create new WikiPage
                WikiContent.WikiPage.CreatedBy = WikiContent.CreatedBy;
                WikiContent.WikiPage.CreatedOn = WikiContent.CreatedOn;
                WikiContent.WikiPage.ModifiedBy = WikiContent.CreatedBy;
                WikiContent.WikiPage.ModifiedOn = WikiContent.CreatedOn;
                _db.WikiPage.Add(WikiContent.WikiPage);
                _db.SaveChanges();
                WikiContent.WikiPageId = WikiContent.WikiPage.WikiPageId;
            }

            WikiContent.Content = ConvertWikiLinks(WikiContent);

            // update WikiPage
            var wikipage = _db.WikiPage.Find(WikiContent.WikiPageId);
            if (wikipage != null)
            {
                wikipage.Title = WikiContent.WikiPage.Title;
                wikipage.Tags = FormatTags(WikiContent.WikiPage.Tags);
                wikipage.ModifiedBy = WikiContent.CreatedBy;
                wikipage.ModifiedOn = WikiContent.CreatedOn;
                _db.Entry(wikipage).State = EntityState.Modified;
                _db.SaveChanges();
            }

            // preserve page path
            var pagepath = WikiContent.WikiPage.PagePath;

            // add new WikiContent if content has changed
            var wikicontent = GetWikiContent(WikiContent.WikiPageId, -1);
            if (wikicontent == null || wikicontent.Content != WikiContent.Content)
            {
                WikiContent.WikiPage = null; // detach WikiPage entity
                _db.WikiContent.Add(WikiContent);
                _db.SaveChanges();
            }

            // process WikiLinks
            ManageWikiLinks(WikiContent, pagepath);

            return WikiContent;
        }

        private string ConvertWikiLinks(WikiContent WikiContent)
        {
            var content = WikiContent.Content;

            // wiki links are in the form [[title]]
            var links = new List<string>();
            int index = content.IndexOf("[[", StringComparison.OrdinalIgnoreCase);
            while (index != -1)
            {
                if (content.IndexOf("]]", index, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    links.Add(content.Substring(index + 2, content.IndexOf("]]", index, StringComparison.OrdinalIgnoreCase) - index - 2));
                }
                index = content.IndexOf("[[", index + 1, StringComparison.OrdinalIgnoreCase);
            }

            if (links.Count > 0)
            {
                var wikicontents = GetWikiContents(WikiContent.WikiPage.ModuleId, "", "").ToList();

                foreach (var link in links)
                {
                    var wikicontent = wikicontents.FirstOrDefault(item => string.Equals(item.WikiPage.Title, link, StringComparison.OrdinalIgnoreCase));

                    if (wikicontent == null)
                    {
                        // create new WikiPage
                        var wikipage = new WikiPage();
                        wikipage.ModuleId = WikiContent.WikiPage.ModuleId;
                        wikipage.Title = link;
                        wikipage.Tags = "";
                        wikipage.CreatedBy = WikiContent.CreatedBy;
                        wikipage.CreatedOn = WikiContent.CreatedOn;
                        wikipage.ModifiedBy = WikiContent.CreatedBy;
                        wikipage.ModifiedOn = WikiContent.CreatedOn;
                        _db.WikiPage.Add(wikipage);
                        _db.SaveChanges();

                        // create WikiContent for WikiPage
                        wikicontent = new WikiContent();
                        wikicontent.WikiPageId = wikipage.WikiPageId;
                        wikicontent.Content = $"<p>{link}</p>";
                        wikicontent.CreatedBy = WikiContent.CreatedBy;
                        wikicontent.CreatedOn = WikiContent.CreatedOn;
                        _db.WikiContent.Add(wikicontent);
                        _db.SaveChanges();
                    }

                    // replace wiki link with hyperlink
                    var parameters = Utilities.AddUrlParameters(wikicontent.WikiPageId, Common.FormatSlug(wikicontent.WikiPage.Title));
                    content = content.Replace($"[[{link}]]", $"<a href=\"{Utilities.NavigateUrl(WikiContent.WikiPage.AliasPath, WikiContent.WikiPage.PagePath, parameters)}\">{link}</a>");
                }
            }

            return content;
        }

        private string FormatTags(string Tags)
        {
            if (!string.IsNullOrEmpty(Tags))
            {
                var tags = Tags.Split(",", StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < tags.Length; i++)
                {
                    tags[i] = tags[i].Trim().ToLower();
                }
                Tags = string.Join(",", tags);
            }
            return Tags;
        }

        private void ManageWikiLinks(WikiContent WikiContent, string PagePath)
        {
            // hyperlinks are in the form PagePath/!/##/Title
            var ids = new List<int>();
            var prefix = PagePath + "/!/";
            int index = WikiContent.Content.IndexOf(prefix);
            while (index != -1)
            {
                if (WikiContent.Content.IndexOf("/", index + prefix.Length) != -1)
                {
                    ids.Add(int.Parse(WikiContent.Content.Substring(index + prefix.Length, WikiContent.Content.IndexOf("/", index + prefix.Length) - index - prefix.Length)));
                }
                index = WikiContent.Content.IndexOf(prefix, index + 1);
            }

            // get current WikiLinks
            var wikilinks = _db.WikiLink.Where(item => item.FromWikiPageId == WikiContent.WikiPageId).ToList();

            foreach (var id in ids)
            {
                var wikilink = wikilinks.FirstOrDefault(item => item.ToWikiPageId == id);
                if (wikilink == null)
                {
                    // add new WikiLink
                    wikilink = new WikiLink();
                    wikilink.FromWikiPageId = WikiContent.WikiPageId;
                    wikilink.ToWikiPageId = id;
                    wikilink.CreatedBy = WikiContent.CreatedBy;
                    wikilink.CreatedOn = WikiContent.CreatedOn;
                    _db.WikiLink.Add(wikilink);
                    _db.SaveChanges();
                }
                else
                {
                    wikilinks.Remove(wikilink);
                }
            }

            // remaining WikiLinks are orphans
            foreach (var wikilink in wikilinks)
            {
                _db.WikiLink.Remove(wikilink);
                _db.SaveChanges();
            }
        }

        public void DeleteWikiContent(int WikiContentId)
        {
            WikiContent WikiContent = _db.WikiContent.Find(WikiContentId);
            _db.WikiContent.Remove(WikiContent);
            _db.SaveChanges();
        }

        public WikiPage GetWikiPage(int WikiPageId)
        {
            return _db.WikiPage.Find(WikiPageId);
        }

        public void DeleteWikiPage(int WikiPageId)
        {
            WikiPage WikiPage = _db.WikiPage.Find(WikiPageId);
            _db.WikiPage.Remove(WikiPage);
            _db.SaveChanges();

            foreach (var wikilink in GetWikiLinks(WikiPageId))
            {
                _db.WikiLink.Remove(wikilink);
                _db.SaveChanges();
            }
        }

        public IEnumerable<WikiLink> GetWikiLinks(int ToWikiPageId)
        {
            return _db.WikiLink.Where(item => item.ToWikiPageId == ToWikiPageId);
        }
    }
}
