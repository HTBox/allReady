using System.Collections.Generic;
using AllReady.ViewModels.Campaign;

namespace AllReady.ViewModels.Home
{
    public class Index
    {
        public CampaignSummaryViewModel FeaturedCampaign { get; set; }
        public List<CampaignViewModel> Campaigns { get; set; }
    }
}
