using System;
using System.ComponentModel.DataAnnotations;
using AllReady.Models;

namespace AllReady.Areas.Admin.Models
{
    public class CampaignSummaryModel: IPrimaryContactModel
    {
        public CampaignSummaryModel()
        {
            this.CampaignImpact = new CampaignImpact();
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Full Description")]
        public string FullDescription { get; set; }

        [Display(Name = "External Link")]
        [Url]
        public string ExternalUrl { get; set; }

        [Display(Name = "External Link Text")]
        public string ExternalUrlText { get; set; }


        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Display(Name = "Organization")]
        public string OrganizationName { get; set; }

        public string ImageUrl { get; set; }

        [Display(Name = "Browse for image")]
        public string FileUpload { get; set; }
            
        [Display(Name = "Time Zone")]
        [Required]
        public string TimeZoneId { get; set; }

        [Display(Name = "Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTimeOffset StartDate { get; set; }

        [Display(Name = "End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTimeOffset EndDate { get; set; }

        public CampaignImpact CampaignImpact { get; set; }

        [UIHint("Location")]
        public LocationEditModel Location { get; set; }

        [Display(Name = "First Name")]
        public string PrimaryContactFirstName { get; set; }

        [Display(Name = "Last Name")]
        public string PrimaryContactLastName { get; set; }

        [Display(Name = "Phone Number")]
        [Phone]
        public string PrimaryContactPhoneNumber { get; set; }

        [MaxLength(150)]
        public string Headline { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string PrimaryContactEmail { get; set; }

        public bool Locked { get; set; }

        public bool Featured { get; set; }
    }
}
