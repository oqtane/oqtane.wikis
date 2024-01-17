using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oqtane.Wiki.Models
{
    public class WikiPage
    {
        public int WikiPageId { get; set; }
        public int ModuleId { get; set; }
        public string Title { get; set; }
        public string Tags { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

        // only used when creating new records
        [NotMapped]
        public string AliasPath { get; set; }
        [NotMapped]
        public string PagePath { get; set; }
    }
}
