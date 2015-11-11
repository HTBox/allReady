using AllReady.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Models
{
    public class CampaignSummaryModel
    {
        public CampaignSummaryModel()
        {
            this.CampaignImpact = new CampaignImpact();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Full Description")]
        public string FullDescription { get; set; }

        [Display(Name = "Organization")]
        public int TenantId { get; set; }

        [Display(Name = "Organization")]
        public string TenantName { get; set; }

        [Display(Name = "Browse for image")]
        public string ImageUrl { get; set; }
            
        [Display(Name = "Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime EndDate { get; set; }

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

        [Display(Name = "Email")]
        [EmailAddress]
        public string PrimaryContactEmail { get; set; }

    }
}
