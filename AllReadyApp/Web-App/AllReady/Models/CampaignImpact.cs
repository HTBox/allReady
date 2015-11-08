using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{
    public class CampaignImpact
    {
        public int Id { get; set; }
        public Campaign Campaign { get; set; }
        public CampaignImpactType CampaignImpactType { get; set; }
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
    }
}
