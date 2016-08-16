using System.Collections.Generic;

namespace AllReady.Areas.Admin.ViewModels.Campaign
{
    public class CampaignEventsViewModel
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public IEnumerable<AllReady.Models.Event> Events { get; set; }
    }
}
