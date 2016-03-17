using System;
using System.ComponentModel.DataAnnotations;
using AllReady.Models;

namespace AllReady.Areas.Admin.Models
{
    public class ActivitySummaryModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, 99, ErrorMessage = "A valid 'Activity Type' is required")]
        [Display(Name = "Activity Type")]
        public ActivityTypes ActivityType { get; set; }

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

        [Range(1, int.MaxValue, ErrorMessage = "'Volunteers Required' must be greater than 0")]
        [Display(Name = "Volunteers Required")]
        public int NumberOfVolunteersRequired { get; set; }
        
        public string TimeZoneId { get; set; }

        [Display(Name = "Start Date")]
        public DateTimeOffset StartDateTime { get; set; }
        [Display(Name = "End Date")]
        public DateTimeOffset EndDateTime { get; set; }

    }
}
