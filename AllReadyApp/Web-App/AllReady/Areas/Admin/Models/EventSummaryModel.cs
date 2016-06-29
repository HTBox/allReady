using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Models;

namespace AllReady.Areas.Admin.Models
{
    public class EventSummaryModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, 99, ErrorMessage = "A valid 'event type' is required")]
        [Display(Name = "Event Type")]
        public EventType EventType { get; set; }

        public string Description { get; set; }

        [Display(Name = "Campaign")]
        public int CampaignId { get; set; }
        [Display(Name = "Campaign")]
        public string CampaignName { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Display(Name = "Organization")]
        public string OrganizationName { get; set; }

        public string ImageUrl { get; set; }

        [Display(Name = "Browse for image")]
        public string FileUpload { get; set; }

        public string TimeZoneId { get; set; }

        [MaxLength(150)]
        public string Headline { get; set; }

        [Display(Name = "Start Date")]
        public DateTimeOffset StartDateTime { get; set; }
        [Display(Name = "End Date")]
        public DateTimeOffset EndDateTime { get; set; }

        [Display(Name = "Allow Wait List")]
        public List<EventSignup> UsersSignedUp { get; set; } = new List<EventSignup>();
        public int NumberOfUsersSignedUp => UsersSignedUp.Count;
    }
}
