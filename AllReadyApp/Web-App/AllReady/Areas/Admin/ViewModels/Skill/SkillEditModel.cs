using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AllReady.Areas.Admin.ViewModels.Skill
{
    public class SkillEditModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public int? OwningOrganizationId { get; set; }

        [Display(Name = "Organization")]
        public string OwningOrganizationName { get; set; }

        [Display(Name = "Parent Skill")]
        public int? ParentSkillId { get; set; }

        public IEnumerable<SkillSummaryModel> ParentSelection { get; set; }

        public IEnumerable<SelectListItem> OrganizationSelection { get; set; }
    }
}