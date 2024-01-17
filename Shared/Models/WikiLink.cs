using System;

namespace Oqtane.Wiki.Models
{
    public class WikiLink
    {
        public int WikiLinkId { get; set; }
        public int FromWikiPageId { get; set; }
        public int ToWikiPageId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
