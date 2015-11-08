using AllReady.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Models
{
    public class TaskSummaryModel
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        [Display(Name = "Activity")]
        public string ActivityName { get; set; }
        public int CampaignId { get; set; }
        [Display(Name = "Campaign")]
        public string CampaignName { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Display(Name = "Start date")]
        public DateTimeOffset? StartDateTime { get; set; }
        [Display(Name = "End date")]
        public DateTimeOffset? EndDateTime { get; set; }

        public bool IsUserSignedUpForTask { get; set; }

        [Display(Name = "Volunteers Required")]
        public int NumberOfVolunteersRequired { get; set; }

        public List<TaskSignupViewModel> AssignedVolunteers { get; set; } = new List<TaskSignupViewModel>();

        public int AcceptedVolunteerCount => AssignedVolunteers?.Where(v => v.Status == "Accepted").Count() ?? 0;
    }
}
