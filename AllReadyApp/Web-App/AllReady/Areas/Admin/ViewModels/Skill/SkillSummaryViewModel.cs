using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Skill
{
    /// <summary>
    /// A view model for display the summary details for a skill
    /// </summary>
    public class SkillSummaryViewModel
    {
        /// <summary>
        ///  The id of the skill
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The hierarchical name of the skill including all direct ancestors
        /// </summary>
        [Display(Name = "Name")]
        public string HierarchicalName { get; set; }

        /// <summary>
        /// The description of the skill
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The name of the organization to which the skill belongs.
        /// </summary>
        [Display(Name = "Owning organization")]
        public string OwningOrganizationName { get; set; }

        /// <summary>
        /// A list ids for all descendants of the skill
        /// </summary>
        public List<int> DescendantIds { get; set; }
    }
}