using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class SkillEditModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Organization")]
        public int OwningOrganizationId { get; set; }

        [Display(Name = "Organization")]
        public string OwningOrganizationName { get; set; }

        public IEnumerable<SkillParentSelectModel> ParentSelectOptions { get; set; }
    }

    public class SkillParentSelectModel
    {
        public int Id { get; set; }
        public string HierarchicalName { get; set; }
    }
}