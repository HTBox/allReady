using AllReady.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AllReady.Areas.Admin.Models
{  
    public class TaskSummaryModel
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

        public bool IsUserSignedUpForTask { get; set; }

        [Display(Name = "Volunteers Required")]
        [Range(1, int.MaxValue, ErrorMessage = "'Volunteers Required' must be greater than 0")]
        public int NumberOfVolunteersRequired { get; set; }

        public List<VolunteerModel> AssignedVolunteers { get; set; } = new List<VolunteerModel>();

        public List<VolunteerModel> AllVolunteers { get; set; } = new List<VolunteerModel>();

        public List<TaskSkill> RequiredSkills { get; set; } = new List<TaskSkill>();

        public int AcceptedVolunteerCount => AssignedVolunteers?.Count(v => v.HasVolunteered) ?? 0;
        public int NumberOfVolunteersSignedUp => AssignedVolunteers.Count;
    }
}
