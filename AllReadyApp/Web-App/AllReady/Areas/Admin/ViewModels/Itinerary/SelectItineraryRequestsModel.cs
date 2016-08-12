using System;
using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Request;

namespace AllReady.Areas.Admin.ViewModels.Itinerary
{
    public class SelectItineraryRequestsModel
    {
        public string ItineraryName { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public DateTime DateAddedUtc { get; set; }

        public string KeywordsFilter { get; set; }

        public RequestSearchCriteria Criteria { get; set; }
        public List<RequestSelectModel> Requests { get; set; } = new List<RequestSelectModel>();
    }
}
