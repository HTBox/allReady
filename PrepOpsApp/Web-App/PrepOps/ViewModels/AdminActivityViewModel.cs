using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrepOps.ViewModels
{
    public class AdminActivityViewModel
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public LocationViewModel Location { get; set; }
        public List<TaskViewModel> Tasks { get; set; }

        public List<string> Volunteers { get; set; }

    }
}
