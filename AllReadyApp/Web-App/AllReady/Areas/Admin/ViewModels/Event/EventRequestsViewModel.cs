using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Itinerary;

namespace AllReady.Areas.Admin.ViewModels.Event
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
        public List<RequestListViewModel> Requests { get; set; } = new List<RequestListViewModel>();

        public string PageTitle { get; set; }
    }
}
