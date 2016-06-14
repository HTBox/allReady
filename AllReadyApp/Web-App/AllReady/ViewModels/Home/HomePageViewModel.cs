using System.Collections.Generic;
using AllReady.ViewModels.Campaign;

namespace AllReady.ViewModels.Home
{
    public class HomePageViewModel
    {
        public List<CampaignViewModel> Campaigns { get; set; }
        public CampaignSummaryViewModel FeaturedCampaign { get; set; }

        public bool HasFeaturedCampaign
        {
            get
            {
                return FeaturedCampaign != null;               
            }
        }
    }
}