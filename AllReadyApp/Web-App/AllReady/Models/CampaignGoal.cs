using System;

namespace AllReady.Models
{
    public class CampaignGoal
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }

        public GoalType GoalType { get; set; }
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
        /// is displayed. This value is also displayed (if not empty)
        /// as a descriptor underneath the thermometer display if the
        /// goal type is numeric.
        /// </summary>
        public string TextualGoal { get; set; }
        public bool Display { get; set; }
        public int PercentComplete
        {
            get
            {
                if (NumericGoal > 0)
                {
                    return (int)Math.Round(((CurrentGoalLevel * 100.0f) / NumericGoal));
                }
                return 0;
            }

        }
    }
}
