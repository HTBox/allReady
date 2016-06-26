using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Models;

namespace AllReady.Areas.Admin.Models
{
    public class CampaignDetailModel: IPrimaryContactModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string FullDescription { get; set; }

        [Display(Name = "External URL")]
        public string ExternalUrl { get; set; }

        [Display(Name = "External URL Text")]
        public string ExternalUrlText { get; set; }


        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }


        [Display(Name = "Organization")]
        public string OrganizationName { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }
            
        [Display(Name = "Time Zone")]
        [Required]
        public string TimeZoneId { get; set; }

        [Display(Name = "Start Date")]
        public DateTimeOffset StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTimeOffset EndDate { get; set; }

		public IEnumerable<EventSummaryModel> Events { get; set; }
        public CampaignImpact CampaignImpact { get; set; }

        [UIHint("Location")]
        public LocationDisplayModel Location { get; set; }
        [Display(Name = "First Name")]
        public string PrimaryContactFirstName { get; set; }

        [Display(Name = "Last Name")]
        public string PrimaryContactLastName { get; set; }

        [Display(Name = "Phone Number")]
        [Phone]
        public string PrimaryContactPhoneNumber { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string PrimaryContactEmail { get; set; }

        public bool Locked { get; set; }

        public bool Featured { get; set; }
    }
}
