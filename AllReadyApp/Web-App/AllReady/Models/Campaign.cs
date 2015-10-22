using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllReady.Models
{
    public class Campaign
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Short synopsis of this campaign, displayed within grids.
        /// </summary>
        public string Description { get; set; }

        [Display(Name = "Full Description")]
        public string FullDescription { get; set; }

        [Display(Name = "Managing Tenant")]
        public int ManagingTenantId { get; set; }

        public Tenant ManagingTenant { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Collection of Tenants that are supporting this campaign
        /// </summary>
        public List<CampaignSponsors> ParticipatingTenants { get; set; }

        /// <summary>
        /// The date the campaign starts
        /// </summary>
        [Display(Name = "Start date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime StartDateTimeUtc { get; set; }

        /// <summary>
        /// The date the campaign ends
        /// </summary>
        [Display(Name = "End date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime EndDateTimeUtc { get; set; }

        public List<Activity> Activities { get; set; }

        public ApplicationUser Organizer { get; set; }


    }
}