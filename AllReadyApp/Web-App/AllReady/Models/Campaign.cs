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

        [Display(Name = "External Link")]
        [Url]
        public string ExternalUrl { get; set; }

        [Display(Name = "External Link Text")]
        public string ExternalUrlText { get; set; }

        /// <summary>
        /// A short piece of optional text which organizers can use to help generate views/volunteers
        /// </summary>
        [MaxLength(150)]
        public string Headline { get; set; }

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

        public List<Event> Events { get; set; }

        public ApplicationUser Organizer { get; set; }

        public List<CampaignGoal> CampaignGoals { get; set; }

        public Location Location { get; set; }

        public List<CampaignContact> CampaignContacts { get; set; } = new List<CampaignContact>();

        public List<Resource> Resources { get; set; } = new List<Resource>();

        public bool Locked { get; set; }

        public bool Featured { get; set; }
        public bool Published { get; set; }

        /// <summary>
        /// Navigation to users who can manage this campaign
        /// </summary>
        public List<CampaignManager> CampaignManagers { get; set; }

        /// <summary>
        /// Navigation property to an invited campaign managers
        /// </summary>
        public List<CampaignManagerInvite> ManagementInvites { get; set; }
    }
}