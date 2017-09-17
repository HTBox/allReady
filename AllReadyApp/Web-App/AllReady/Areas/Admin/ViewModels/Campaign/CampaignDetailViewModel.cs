using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Areas.Admin.ViewModels.Organization;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.Campaign
{
    public class CampaignDetailViewModel : IPrimaryContactViewModel
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

        public IEnumerable<EventList> Events { get; set; }
        public IEnumerable<ResourceList> Resources { get; set; }
        public IEnumerable<CampaignManagerInviteList> CampaignManagerInvites { get; set; }

        public List<CampaignGoal> CampaignGoals { get; set; }

        [UIHint("Location")]
        public LocationDisplayViewModel Location { get; set; }

        [Display(Name = "First Name")]
        public string PrimaryContactFirstName { get; set; }

        [Display(Name = "Last Name")]
        public string PrimaryContactLastName { get; set; }

        [Display(Name = "Mobile phone Number")]
        [Phone]
        public string PrimaryContactPhoneNumber { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string PrimaryContactEmail { get; set; }

        public bool Locked { get; set; }

        public bool Featured { get; set; }

        public bool Published { get; set; }

        public class EventList
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            [Display(Name = "Start Date")]
            public DateTimeOffset StartDateTime { get; set; }

            [Display(Name = "End Date")]
            public DateTimeOffset EndDateTime { get; set; }
        }

        public class ResourceList
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
        }

        public class CampaignManagerInviteList
        {
            public int Id { get; set; }
            public string InviteeEmail { get; set; }
            public CampaignManagerInviteStatus Status { get; set; }
        }

        public enum CampaignManagerInviteStatus
        {
            Pending,
            Accepted,
            Rejected,
            Revoked,
        }
    }
}