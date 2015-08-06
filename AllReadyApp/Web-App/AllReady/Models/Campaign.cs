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

        public string Name { get; set; }

        public string Description { get; set; }

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
        public DateTime StartDateTimeUtc { get; set; }

        /// <summary>
        /// The date the campaign ends
        /// </summary>
        [Display(Name = "End date")]
        public DateTime EndDateTimeUtc { get; set; }

        public List<Activity> Activities { get; set; }

        public ApplicationUser Organizer { get; set; }


    }
}