using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.ViewModels
{
    public class CampaignSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public int TenantId { get; set; }
        public string TenantName
        {
            get; set;
        }

        public string ImageUrl { get; set; }
            
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
