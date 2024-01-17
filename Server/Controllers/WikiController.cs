using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Wiki.Repository;
using Microsoft.AspNetCore.Http;
using Oqtane.Controllers;
using Oqtane.Repository;
using Oqtane.Models;
using Oqtane.Wiki.Shared;
using Oqtane.Extensions;
using System.Linq;
using System;
using System.Net;
using Oqtane.Wiki.Models;

namespace Oqtane.Wiki.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class WikiController : ModuleControllerBase
    {
        private readonly IWikiRepository _WikiRepository;
        private readonly ISiteRepository _SiteRepository;
        private readonly ISettingRepository _SettingRepository;

        public WikiController(IWikiRepository WikiRepository, ISiteRepository SiteRepository, ISettingRepository SettingRepository, ILogManager logger, IHttpContextAccessor accessor) : base(logger,accessor)
        {
            _WikiRepository = WikiRepository;
            _SiteRepository = SiteRepository;
            _SettingRepository = SettingRepository;
        }

        // GET: api/<controller>/search?moduleid=x&search=y&tag=z
        [HttpGet("search")]
        [Authorize(Policy = "ViewModule")]
        public IEnumerable<WikiContent> Search(string moduleid, string search, string tag)
        {
            if (int.TryParse(moduleid, out int ModuleId) && IsAuthorizedEntityId(EntityNames.Module, ModuleId))
            {
                return _WikiRepository.GetWikiContents(ModuleId, search, tag);
            }
            else
            {
                return null;
            }
        }
        
        // GET: api/<controller>?moduleid=x&wikipageid=y
        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public IEnumerable<WikiContent> Get(string moduleid, string wikipageid)
        {
            if (int.TryParse(moduleid, out int ModuleId) && IsAuthorizedEntityId(EntityNames.Module, ModuleId) && int.TryParse(wikipageid, out int WikiPageId))
            {
                return _WikiRepository.GetWikiContents(ModuleId, WikiPageId);
            }
            else
            {
                return null;
            }
        }
        
        // GET api/<controller>/5
        [HttpGet("{wikipageid}")]
        [Authorize(Policy = "ViewModule")]
        public WikiContent Get(int wikipageid)
        {
            WikiContent WikiContent = _WikiRepository.GetWikiContent(wikipageid, -1);
            if (WikiContent != null && IsAuthorizedEntityId(EntityNames.Module, WikiContent.WikiPage.ModuleId))
            {
                // get wiki links
                WikiContent.Links = new Dictionary<int, string>();
                foreach (var wikilink in _WikiRepository.GetWikiLinks(wikipageid).ToList())
                {
                    var wikicontent = _WikiRepository.GetWikiContent(wikilink.FromWikiPageId, -1);
                    if (wikicontent != null)
                    {
                        WikiContent.Links.Add(wikilink.FromWikiPageId, wikicontent.WikiPage.Title);
                    }
                }
            }
            else
            {
                WikiContent = null;
            }
            return WikiContent;
        }

        // GET api/<controller>/5/6
        [HttpGet("{wikipageid}/{id}")]
        [Authorize(Policy = "ViewModule")]
        public WikiContent Get(int wikipageid, int id)
        {
            WikiContent WikiContent = _WikiRepository.GetWikiContent(wikipageid, id);
            if (WikiContent != null && IsAuthorizedEntityId(EntityNames.Module, WikiContent.WikiPage.ModuleId))
            {
                WikiContent = null;
            }
            return WikiContent;
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = "EditModule")]
        public WikiContent Post([FromBody] WikiContent WikiContent)
        {
            if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, WikiContent.WikiPage.ModuleId))
            {
                WikiContent = _WikiRepository.AddWikiContent(WikiContent);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "WikiContent Added {WikiContent}", WikiContent);
            }
            return WikiContent;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{wikipageid}/{id}")]
        [Authorize(Roles = RoleNames.Admin)]
        public void Delete(int wikipageid, int id)
        {
            WikiContent WikiContent = _WikiRepository.GetWikiContent(wikipageid, id);
            if (WikiContent != null)
            {
                _WikiRepository.DeleteWikiContent(id);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "WikiContent Deleted {WikiContentId}", id);
            }
        }

        // DELETE api/<controller>/page/5
        [HttpDelete("page/{id}")]
        [Authorize(Roles = RoleNames.Admin)]
        public void DeletePage(int id)
        {
            WikiPage WikiPage = _WikiRepository.GetWikiPage(id);
            if (WikiPage != null)
            {
                _WikiRepository.DeleteWikiPage(id);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "WikiPage Deleted {WikiPageId}", id);
            }
        }

        [HttpGet("rss/{id}")]
        public IActionResult RSS(int id)
        {
            var alias = HttpContext.GetAlias();
            var rooturl = alias.Protocol + alias.Name;

            var site = _SiteRepository.GetSite(alias.SiteId);
            var settings = _SettingRepository.GetSettings(EntityNames.Module, id);
            var pagepath = "/" + settings.First(item => item.SettingName == "PagePath").SettingValue;

            var rss = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" + Environment.NewLine;
            rss += "<rss version=\"2.0\">" + Environment.NewLine;
            rss += "<channel>" + Environment.NewLine;
            rss += "<title>" + WebUtility.HtmlEncode(site.Name) + "</title>" + Environment.NewLine;
            rss += "<link>" + rooturl + pagepath + "</link>" + Environment.NewLine;
            rss += "<description>" + WebUtility.HtmlEncode(site.Name) + "</description>" + Environment.NewLine;

            foreach (var WikiContent in _WikiRepository.GetWikiContents(id, "", ""))
            {
                rss += "<item>" + Environment.NewLine;
                rss += "<title>" + WebUtility.HtmlEncode(WikiContent.WikiPage.Title) + "</title>" + Environment.NewLine;
                var parameters = Utilities.AddUrlParameters(WikiContent.WikiPageId, Common.FormatSlug(WikiContent.WikiPage.Title));
                rss += "<link>" + rooturl + Utilities.NavigateUrl(alias.Path, pagepath, parameters) + "</link>" + Environment.NewLine;
                rss += "<description>" + WebUtility.HtmlEncode(Common.CreateSummary(WikiContent.Content, 200, "")) + "</description>" + Environment.NewLine;
                rss += "<pubDate>" + WikiContent.CreatedOn.ToString("r") + "</pubDate>";
                rss += "</item>" + Environment.NewLine;
            }

            rss += "</channel>" + Environment.NewLine;
            rss += "</rss>" + Environment.NewLine;

            return Content(rss, "application/xml");
        }
    }
}
