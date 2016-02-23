using System.Collections.Generic;
using AllReady.Models;

namespace AllReady.Areas.Admin.Models
{
    public class CampaignActivitiesModel
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public IEnumerable<Activity> Activities { get; set; }
    }
}
