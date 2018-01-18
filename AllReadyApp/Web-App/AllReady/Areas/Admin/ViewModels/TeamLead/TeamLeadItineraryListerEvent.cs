using System.Collections.Generic;

namespace AllReady.Areas.Admin.ViewModels.TeamLead
{
    public class TeamLeadItineraryListerEvent
    {
        public string Name { get; set; }

        public List<TeamLeadItineraryListerItinerary> Itineraries { get; set; } = new List<TeamLeadItineraryListerItinerary>();
    }
}