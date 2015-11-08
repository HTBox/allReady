using AllReady.Models;
using System.Collections.Generic;

namespace AllReady.Areas.Admin.ViewModels
{
    public class CampaignActivitiesViewModel
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public IEnumerable<Activity> Activities { get; set; }
    }
}
