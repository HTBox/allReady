using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class CampaignImpactModel
    {
        public int Id { get; set; }
        /// <summary>
        /// If the impact type is numeric, the value is the number
        /// of x to reach the goal.
        /// </summary>
        public int NumericImpactGoal { get; set; }
        /// <summary>
        /// If the impact type is numeric, the value is the current
        /// number of x achieved to reach the NumericImpactGoal value.
        /// Ideally less than or equal to NumericImpactGoal, but there
        /// may be certain scenarios where it's greater.
        /// </summary>
        public int CurrentImpactLevel { get; set; }
        /// <summary>
        /// If the impact type is textual, the TextualImpactGoal value
        /// is displayed.
        /// </summary>
        public string TextualImpactGoal { get; set; }
        public bool Display { get; set; }

        [Display(Name = "Goal Type")]
        public int CampaignImpactTypeId { get; set; }

        [Display(Name = "Goal Type")]
        public string CampaignImpactTypeName { get; set; }
    }
}
