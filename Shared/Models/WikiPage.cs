using System;

namespace Oqtane.Wiki.Models
{
    public class WikiPage
    {
        public int WikiPageId { get; set; }
        public int ModuleId { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
