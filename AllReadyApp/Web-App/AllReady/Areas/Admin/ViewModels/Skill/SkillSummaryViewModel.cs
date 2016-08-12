using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Skill
{
    public class SkillSummaryViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string HierarchicalName { get; set; }

        public string Description { get; set; }

        [Display(Name = "Owning organization")]
        public string OwningOrganizationName { get; set; }
    }
}