using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oqtane.Wiki.Models
{
    public class WikiContent
    {
        public int WikiContentId { get; set; }
        public int WikiPageId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public WikiPage WikiPage { get; set; }

        // only used during add
        [NotMapped]
        public int ModuleId { get; set; } 
        [NotMapped]
        public string AliasPath { get; set; }
        [NotMapped]
        public string PagePath { get; set; }
    }
}
