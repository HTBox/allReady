using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

        /// <summary>
        /// A navigational property to the child skills of this skill
        /// </summary>
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

        public List<int> DescendantIds
        {
            get
            {
                // NOTE: by sgordon - Whilst we have limited skills in the system this is okay. In time, depending on the growth of skills we should review more efficient ways to get this information
                // and caching of the resulting data to reduce the impact of this requirement.

                if (ChildSkills == null)
                {
                    // If child skills are not loaded we match the return and mark this null
                    return null;
                }

                return !ChildSkills.Any() ? new List<int>() : EvaluateDescendantIds(this);
            }
        }

        private List<int> EvaluateDescendantIds(Skill skill, List<int> descendantIds = null, List<int> priorSkills = null)
        {
            // NOTE: by sgordon - Whilst we have limited skills in the system this is okay. In time, depending on the growth of skills we should review more efficient ways to get this information
            // and caching of the resulting data to reduce the impact of this requirement.

            if (skill == null)
            {
                throw new ArgumentNullException(nameof(skill));
            }

            if (priorSkills == null)
            {
                priorSkills = new List<int> { skill.Id };
            }
            else
            {
                if (priorSkills.Any(x => x == skill.Id))
                {
                    return null; // we safety check we aren't in a perpetual loop caused by an invalid hierarchy using the iteration count
                }
            }

            if (descendantIds == null)
            {
                descendantIds = new List<int>();
            }

            if (skill.ChildSkills != null)
            {
                foreach (var childSkill in skill.ChildSkills)
                {
                    descendantIds.Add(childSkill.Id);

                    EvaluateDescendantIds(childSkill, descendantIds, priorSkills);

                    priorSkills.Add(childSkill.Id);
                }
            }

            return descendantIds;
        }
    }
}