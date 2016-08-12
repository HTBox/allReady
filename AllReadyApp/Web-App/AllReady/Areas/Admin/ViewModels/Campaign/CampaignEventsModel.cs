using System.Collections.Generic;

namespace AllReady.Areas.Admin.ViewModels.Campaign
{
    public class CampaignEventsModel
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public IEnumerable<AllReady.Models.Event> Events { get; set; }
    }
}
