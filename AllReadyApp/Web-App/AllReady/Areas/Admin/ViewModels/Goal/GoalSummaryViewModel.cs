using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.Goal
{
    public class GoalSummaryViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Campaign")]
        public int CampaignId { get; set; }

        [Display(Name = "Campaign")]
        public string CampaignName { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Display(Name = "Organization")]
        public string OrganizationName { get; set; }

        public ImpactType ImpactType { get; set; }

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
        /// is displayed. This value is also displayed (if not empty)
        /// as a descriptor underneath the thermometer display if the
        /// impact type is numeric.
        /// </summary>
        public string TextualImpactGoal { get; set; }
        public bool Display { get; set; }
        public int PercentComplete
        {
            get
            {
                if (NumericImpactGoal > 0)
                {
                    return (int)Math.Round(((CurrentImpactLevel * 100.0f) / NumericImpactGoal));
                }
                return 0;
            }

        }

    }
}
