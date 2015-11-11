using AllReady.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class CampaignDetailModel: IPrimaryContactModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string FullDescription { get; set; }

        [Display(Name = "Organization")]
        public int TenantId { get; set; }


        [Display(Name = "Organization")]
        public string TenantName { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }
            
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

		public IEnumerable<ActivitySummaryModel> Activities { get; set; }
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

    }
}
