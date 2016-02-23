using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Models
{
    public class Campaign
    {
        public int Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Short synopsis of this campaign, displayed within grids.
        /// </summary>
        public string Description { get; set; }
        [Display(Name = "Full Description")]
        public string FullDescription { get; set; }

        [Display(Name = "Managing Organization")]
        public int ManagingOrganizationId { get; set; }
        public Organization ManagingOrganization { get; set; }
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Collection of <see cref="Organization"/>s that are supporting this campaign
        /// </summary>
        public List<CampaignSponsors> ParticipatingOrganizations { get; set; }

        [Display(Name = "Time Zone")]
        [Required]
        public string TimeZoneId { get; set; }

        /// <summary>
        /// The date the campaign starts
        /// </summary>
        [Display(Name = "Start date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTimeOffset StartDateTime { get; set; }

        /// <summary>
        /// The date the campaign ends
        /// </summary>
        [Display(Name = "End date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTimeOffset EndDateTime { get; set; }

        public List<Activity> Activities { get; set; }

        public ApplicationUser Organizer { get; set; }

        public int? CampaignImpactId { get; set; }
        public CampaignImpact CampaignImpact { get; set; }

        public Location Location { get; set; }

        public List<CampaignContact> CampaignContacts { get; set; }

        public bool Locked { get; set; }
    }
}