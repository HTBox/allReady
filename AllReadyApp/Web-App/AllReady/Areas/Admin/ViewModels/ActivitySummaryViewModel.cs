using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.ViewModels
{
    public class ActivitySummaryViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [Display(Name = "Campaign")]
        public int CampaignId { get; set; }
        [Display(Name = "Campaign")]
        public string CampaignName { get; set; }

        [Display(Name = "Organization")]
        public int TenantId { get; set; }

        [Display(Name = "Organization")]
        public string TenantName { get; set; }

        public string ImageUrl { get; set; }

        
        [Display(Name = "Start Date")]
        public DateTime StartDateTime { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDateTime { get; set; }

    }
}
