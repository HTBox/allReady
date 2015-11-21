using AllReady.Models;
using System.Collections.Generic;
using System.Linq;

namespace AllReady.Areas.Admin.Models
{
    public static class CampaignImpactExtensions
    {
        public static CampaignImpact UpdateModel(this CampaignImpact campaignImpact, CampaignImpact editModel)
        {
            if (editModel != null)
            {
                if (campaignImpact == null || editModel.Id == 0)
                {
                    campaignImpact = new CampaignImpact();
                }
                campaignImpact.ImpactType = editModel.ImpactType;
                campaignImpact.CurrentImpactLevel = editModel.CurrentImpactLevel;
                campaignImpact.Display = editModel.Display;
                campaignImpact.NumericImpactGoal = editModel.NumericImpactGoal;
                campaignImpact.TextualImpactGoal = editModel.TextualImpactGoal;

                return campaignImpact;
            }

            return campaignImpact;

        }
    }
}
