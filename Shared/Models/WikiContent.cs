using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oqtane.Wiki.Models
{
    public class WikiContent
    {
        public int WikiContentId { get; set; }
        public int WikiPageId { get; set; }
        public string Content { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public WikiPage WikiPage { get; set; }

        [NotMapped]
        public Dictionary<int, string> Links { get; set; }

    }
}
