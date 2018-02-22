using System;
using System.ComponentModel.DataAnnotations;
using AllReady.Models;
using AllReady.ModelBinding;

namespace AllReady.Areas.Admin.ViewModels.Shared
{
    public class EventSummaryViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Event Title")]
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

        [Display(Name = "Existing Image")]
        public string ImageUrl { get; set; }

        [Display(Name = "Upload event image")]
        public string FileUpload { get; set; }        

        [MaxLength(150)]
        public string Headline { get; set; }

        [Display(Name = "Time Zone")]
        [Required]
        public string TimeZoneId { get; set; }

        [Display(Name = "Start Date")]
        [AdjustToTimezone(TimeZoneIdPropertyName = nameof(TimeZoneId))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}")]
        //What do I nee to go here to format this as a local date time when viewing it? I don't want the offset to be sent to the client because
        //that just confuses things
        public DateTimeOffset StartDateTime { get; set; }

        [Display(Name = "End Date")]
        [AdjustToTimezone(TimeZoneIdPropertyName = nameof(TimeZoneId))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}")]
        public DateTimeOffset EndDateTime { get; set; }

        [Display(Name = "Enforce volunteer limit on tasks")]
        public bool IsLimitVolunteers { get; set; } = true;

        // NOTE: stevejgordon - 31-08-16 - Defaulting to false at part of work on #919 since the code to ensure this is working doesn't seem to be fully in place.
        [Display(Name = "Allow waiting list for tasks")]
        public bool IsAllowWaitList { get; set; }
    }
}
