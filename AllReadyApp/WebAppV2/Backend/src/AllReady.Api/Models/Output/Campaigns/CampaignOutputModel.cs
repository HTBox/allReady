using AllReady.Api.Models.Output.Events;
using AllReady.Api.Models.Output.Locations;
using System.Collections.Generic;

namespace AllReady.Api.Models.Output.Campaigns
{
    public class CampaignOutputModel : CampaignOutputModelBase
    {
        public string FullDesription { get; set; }

        public LocationOutputModel Location { get; set; }

        public IEnumerable<EventListerOutputModel> Events { get; set; }
    }
}
