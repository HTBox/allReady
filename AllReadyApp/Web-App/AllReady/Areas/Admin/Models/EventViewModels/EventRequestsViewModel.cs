using System.Collections.Generic;
using AllReady.Areas.Admin.Models.ItineraryModels;

namespace AllReady.Areas.Admin.Models.EventViewModels
{
    /// <summary>
    /// Defines data used by the admin event request lister page
    /// </summary>
    public class EventRequestsViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }

        public List<RequestListModel> Requests { get; set; } = new List<RequestListModel>();

        public string PageTitle { get; set; }
    }
}
