using System.Collections.Generic;

namespace AllReady.Areas.Admin.ViewModels.TeamLead
{
    public class TeamLeadItineraryListerCampaign
    {
        public string Name { get; set; }

        public List<TeamLeadItineraryListerEvent> CampaignEvents { get; set; } = new List<TeamLeadItineraryListerEvent>();
    }
}