using System.Collections.Generic;

namespace AllReady.ViewModels.Home
{
    public class IndexViewModel
    {
        public List<ActiveOrUpcomingCampaign> ActiveOrUpcomingCampaigns { get; set; }
        public CampaignSummaryViewModel FeaturedCampaign { get; set; }
        public bool HasFeaturedCampaign => FeaturedCampaign != null;
    }
}