using AllReady.Api.Models.Output.Events;
using System.Collections.Generic;

namespace AllReady.Api.Models.Output.Campaigns
{
    public class CampaignOutputModel : CampaignOutputModelBase
    {
        public string FullDesription { get; set; }

        public IEnumerable<EventListerOutputModel> Events { get; set; }
    }
}
