﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Models;

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
        public DateTimeOffset StartDateTime { get; set; }

        [Display(Name = "End date")]
        public DateTimeOffset EndDateTime { get; set; }

        [Display(Name = "Volunteers Required")]
        [Range(1, int.MaxValue, ErrorMessage = "'Volunteers Required' must be greater than 0")]
        public int NumberOfVolunteersRequired { get; set; }

        public List<TaskSkill> RequiredSkills { get; set; } = new List<TaskSkill>();

        //only used for update sceanrios
        public List<VolunteerViewModel> AssignedVolunteers { get; set; } = new List<VolunteerViewModel>();

        //added for min/max validation on the create/edit views
        public DateTimeOffset EventStartDateTime { get; set; }

        //added for min/max validation on the create/edit views
        public DateTimeOffset EventEndDateTime { set; get; }

        //used to build Cancel button url for create and edit actions
        public string CancelUrl { get; set; }
    }
}