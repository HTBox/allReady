using AllReady.Models;

namespace AllReady.Areas.Admin.Models
{
    public static class CampaignGoalExtensions
    {
        public static CampaignGoal UpdateModel(this CampaignGoal campaignGoal, CampaignGoal editModel)
        {
            if (editModel != null)
            {
                if (campaignGoal == null || editModel.Id == 0)
                {
                    campaignGoal = new CampaignGoal();
                }
                campaignGoal.GoalType = editModel.GoalType;
                campaignGoal.CurrentGoalLevel = editModel.CurrentGoalLevel;
                campaignGoal.Display = editModel.Display;
                campaignGoal.NumericGoal = editModel.NumericGoal;
                campaignGoal.TextualGoal = editModel.TextualGoal;

                return campaignGoal;
            }

            return campaignGoal;

        }
    }
}
