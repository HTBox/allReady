using AllReady.Models;
using AllReady.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.ViewModels
{
    public class ActivityDetailViewModel
    {
        public int Id { get; set; }

        [Display(Name ="Organization")]
        public int TenantId { get; set; }

        [Display(Name = "Organization")]
        public string TenantName { get; set; }

        [Display(Name = "Campaign")]
        public int CampaignId { get; set; }
        [Display(Name = "Campaign")]
        public string CampaignName { get; set; }

        [Display(Name = "Name")]
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDateTime { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDateTime { get; set; }
        public LocationViewModel Location { get; set; }
        public IEnumerable<TaskSummaryViewModel> Tasks { get; set; }
        public List<string> Volunteers { get; set; }

        [Display(Name = "Required Skills")]
        public IEnumerable<ActivitySkill> RequiredSkills { get; set; }
    }
}
