using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Skill
{
    public class SkillDeleteViewModel
    {
        [Display(Name = "Name")]
        public string HierarchicalName { get; set; }

        public int? OwningOrganizationId { get; set; }

        [Display(Name = "Children")]
        public IEnumerable<string> ChildrenNames { get; set; }
    }
}