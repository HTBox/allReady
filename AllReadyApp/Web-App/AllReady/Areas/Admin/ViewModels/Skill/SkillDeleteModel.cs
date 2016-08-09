using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class SkillDeleteModel
    {
        [Display(Name = "Name")]
        public string HierarchicalName { get; set; }

        public int? OwningOrganizationId { get; set; }

        [Display(Name = "Children")]
        public IEnumerable<string> ChildrenNames { get; set; }
    }
}