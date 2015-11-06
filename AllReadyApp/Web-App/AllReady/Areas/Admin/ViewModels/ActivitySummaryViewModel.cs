using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.ViewModels
{
    public class ActivitySummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CampaignId { get; set; }

        public string CampainName { get; set; }

        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
