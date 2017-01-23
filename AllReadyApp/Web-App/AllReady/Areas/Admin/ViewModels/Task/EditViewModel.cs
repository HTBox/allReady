using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Models;
using AllReady.ModelBinding;

namespace AllReady.Areas.Admin.ViewModels.Task
{
    public class EditViewModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        [Display(Name = "Event")]
        public string EventName { get; set; }

        public int CampaignId { get; set; }

        [Display(Name = "Campaign")]
        public string CampaignName { get; set; }

        public int OrganizationId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string TimeZoneId { get; set; }

        [Display(Name = "Start date")]
        [AdjustToTimezone(TimeZoneIdPropertyName = nameof(TimeZoneId))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}")]
        public DateTimeOffset StartDateTime { get; set; }

        [Display(Name = "End date")]
        [AdjustToTimezone(TimeZoneIdPropertyName = nameof(TimeZoneId))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}")]
        public DateTimeOffset EndDateTime { get; set; }

        [Display(Name = "Volunteers Required")]
        [Range(1, int.MaxValue, ErrorMessage = "'Volunteers Required' must be greater than 0")]
        public int NumberOfVolunteersRequired { get; set; }

        public List<VolunteerTaskSkill> RequiredSkills { get; set; } = new List<VolunteerTaskSkill>();

        //only used for update scenarios
        public List<VolunteerViewModel> AssignedVolunteers { get; set; } = new List<VolunteerViewModel>();

        //used to build Cancel button url for create and edit actions
        public string CancelUrl { get; set; }
    }
}