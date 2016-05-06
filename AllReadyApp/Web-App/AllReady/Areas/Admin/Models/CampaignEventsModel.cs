using System.Collections.Generic;
using AllReady.Models;

namespace AllReady.Areas.Admin.Models
{
    public class CampaignEventsModel
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public IEnumerable<Event> Events { get; set; }
    }
}
