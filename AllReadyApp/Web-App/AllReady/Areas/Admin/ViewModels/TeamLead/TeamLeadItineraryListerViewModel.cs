using System.Collections.Generic;
using System.Linq;

namespace AllReady.Areas.Admin.ViewModels.TeamLead
{
    public class TeamLeadItineraryListerViewModel
    {
        public List<TeamLeadItineraryListerCampaign> Campaigns { get; set; } = new List<TeamLeadItineraryListerCampaign>();

        public bool HasItineraries => Campaigns.Any();
    }
}
