using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.VolunteerTask
{  
    public class TaskSummaryViewModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        [Display(Name = "Event")]
        public string EventName { get; set; }

        public DateTimeOffset EventStartDateTime { get; set; }

        public DateTimeOffset EventEndDateTime { set; get; }

        public int CampaignId { get; set; }

        [Display(Name = "Campaign")]
        public string CampaignName { get; set; }

        public int OrganizationId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string TimeZoneId { get; set; }

        [Display(Name = "Start date")]
        public DateTimeOffset StartDateTime { get; set; }
        
        [Display(Name = "End date")]
        public DateTimeOffset EndDateTime { get; set; }

        public bool IsUserSignedUpForVolunteerTask { get; set; }

        [Display(Name = "Volunteers Required")]
        [Range(1, int.MaxValue, ErrorMessage = "'Volunteers Required' must be greater than 0")]
        public int NumberOfVolunteersRequired { get; set; }

        public List<VolunteerViewModel> AssignedVolunteers { get; set; } = new List<VolunteerViewModel>();

        public List<VolunteerTaskSkill> RequiredSkills { get; set; } = new List<VolunteerTaskSkill>();

        public int AcceptedVolunteerCount => this.AssignedVolunteers?.Count(v => v.HasVolunteered) ?? 0;

        public int NumberOfVolunteersSignedUp => this.AssignedVolunteers.Count;
    }
}