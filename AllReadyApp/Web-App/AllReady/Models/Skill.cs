using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllReady.Models
{
    public class Skill
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int? OwningOrganizationId { get; set; }
        public Tenant OwningOrganization { get; set; }
        public int? ParentSkillId { get; set; }
        [Display(Name = "Parent skill")]
        public virtual Skill ParentSkill { get; set; }
        public string HierarchicalName
        {
            get
            {
                var retStr = Name;
                var parent = ParentSkill;
                while (parent != null)
                {
                    retStr = $"{parent.Name} > {retStr}";
                    parent = parent.ParentSkill;
                }
                return retStr;
            }
        }
    }
}
