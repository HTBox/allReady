using System.Collections.Generic;

namespace AllReady.ViewModels
{
    public class HomePageViewModel
    {
        public List<CampaignViewModel> Campaigns { get; set; }
        public CampaignSummaryViewModel FeaturedCampaign { get; set; }

        public bool HasFeaturedCampaign
        {
            get
            {
                if (FeaturedCampaign != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}