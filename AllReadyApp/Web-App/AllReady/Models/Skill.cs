using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllReady.Models
{
    /// <summary>
    /// Defines a skill in the allReady application. This is used for EF mapping
    /// </summary>
    public class Skill
    {
        [NotMapped]
        public const string InvalidHierarchy = "Invalid hierarchy";

        /// <summary>
        /// The unique ID of the skill - set by SQL
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of skill
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// A description of the skill
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The owning organization if the skill is not a general skill
        /// </summary>
        public int? OwningOrganizationId { get; set; }

        /// <summary>
        /// The navigation property for the owning organization if this skill has one
        /// </summary>
        [Display(Name = "Owning organization")]
        public Organization OwningOrganization { get; set; }

        /// <summary>
        /// The id of the parent skill. This will be null if the skill is the top level parent
        /// </summary>
        public int? ParentSkillId { get; set; }

        /// <summary>
        /// A navigational property to the parent skill if set
        /// </summary>
        [Display(Name = "Parent skill")]
        public virtual Skill ParentSkill { get; set; }

        public List<Skill> ChildSkills { get; set; }

        /// <summary>
        /// Returns a calculated full hierarchical name for the skill showing it's parent/children. This is not mapped to EF
        /// </summary>
        public string HierarchicalName
        {
            get
            {
                var usedSkills = new List<int> { Id };
                var hierarchicalName = Name;
                var parent = ParentSkill;

                while (parent != null)
                {
                    if (usedSkills.Contains(parent.Id))
                    {
                        // Prevents an infinite hierarchical loop
                        // We shouldn't have a parent being added which has already been included in our hierarchy
                        return InvalidHierarchy;
                    }

                    usedSkills.Add(parent.Id);

                    hierarchicalName = $"{parent.Name} > {hierarchicalName}";
                    parent = parent.ParentSkill;
                }

                return hierarchicalName;
            }
        }
    }
}