using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Campaign
{
    public class CampaignGoalViewModel
    {
        public int Id { get; set; }
        /// <summary>
        /// If the goal type is numeric, the value is the number
        /// of x to reach the goal.
        /// </summary>
        public int NumericGoal { get; set; }
        /// <summary>
        /// If the goal type is numeric, the value is the current
        /// number of x achieved to reach the NumericGoal value.
        /// Ideally less than or equal to NumericGoal, but there
        /// may be certain scenarios where it's greater.
        /// </summary>
        public int CurrentGoalLevel { get; set; }
        /// <summary>
        /// If the goal type is textual, the TextualGoal value
        /// is displayed.
        /// </summary>
        public string TextualGoal { get; set; }
        public bool Display { get; set; }

        [Display(Name = "Goal Type")]
        public int CampaignGoalTypeId { get; set; }

        [Display(Name = "Goal Type")]
        public string CampaignGoalTypeName { get; set; }
    }
}
