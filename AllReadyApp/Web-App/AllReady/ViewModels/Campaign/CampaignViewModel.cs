using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Models;
using AllReady.ViewModels.Event;

namespace AllReady.ViewModels.Campaign
{
    public class CampaignViewModel
    {
        public CampaignViewModel()
        {
            Events = new List<EventViewModel>();
        }

        public CampaignViewModel(Models.Campaign campaign)
        {
            if (campaign == null)
                return;

            Id = campaign.Id;
            Name = campaign.Name;
            Description = campaign.Description;
            FullDescription = campaign.FullDescription;
            ExternalUrl = campaign.ExternalUrl;
            ExternalUrlText = campaign.ExternalUrlText;
            ManagingOrganizationName = campaign.ManagingOrganization?.Name ?? string.Empty;
            ManagingOrganizationId = campaign.ManagingOrganization?.Id ?? 0;
            ManagingOrganizationLogo = campaign.ManagingOrganization?.LogoUrl ?? string.Empty;
            TimeZoneId = campaign.TimeZoneId;
            StartDate = campaign.StartDateTime;
            EndDate = campaign.EndDateTime;            
            Events = campaign.Events != null ? campaign.Events.ToViewModel() : Enumerable.Empty<EventViewModel>();
            CampaignGoals = campaign.CampaignGoals;
            ImageUrl = campaign.ImageUrl;
            HasPrivacyPolicy = !string.IsNullOrEmpty(campaign.ManagingOrganization?.PrivacyPolicy);
            PrivacyPolicyUrl = campaign.ManagingOrganization?.PrivacyPolicyUrl;
            Location = campaign.Location;
            Featured = campaign.Featured;
            Headline = campaign.Headline;
            Published = campaign.Published;

        }

        public CampaignViewModel(CampaignSummaryViewModel from)
        {
            Name = from.Name;
            Description = from.Description;
            FullDescription = from.FullDescription;
            ExternalUrl = from.ExternalUrl;
            ExternalUrlText = from.ExternalUrlText;
            ManagingOrganizationId = from.OrganizationId;
            ManagingOrganizationName = from.OrganizationName;
            ImageUrl = from.ImageUrl;
            TimeZoneId = from.TimeZoneId;
            StartDate = from.StartDate;
            EndDate = from.EndDate;
            Headline = from.Headline;
            Featured = from.Featured;
            Published = from.Published;
            CampaignGoals = new List<CampaignGoal>();
            Location = new Location(from.Location);
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ExternalUrl { get; set; }
        
        public string ExternalUrlText { get; set; }

        public string FullDescription { get; set; }

        public string ImageUrl { get; set; }

        public Location Location { get; set; }

        public string LocationSummary
        {
            get
            {
                if (!string.IsNullOrEmpty(Location?.City) && !string.IsNullOrEmpty(Location?.State))
                    return $"{Location.City}, {Location.State}";
                if (!string.IsNullOrEmpty(Location?.City))
                    return $"{Location.City}";
                if (!string.IsNullOrEmpty(Location?.State))
                    return $"{Location.State}";

                return string.Empty;
            }
        }

        public int ManagingOrganizationId { get; set; }

        public string ManagingOrganizationName { get; set; }

        public string ManagingOrganizationLogo { get; set; }

        public List<CampaignGoal> CampaignGoals { get; set; }

        public List<CampaignSponsors> ParticipatingOrganizations { get; set; }

        public string TimeZoneId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public IEnumerable<EventViewModel> Events { get; set; }

        public string PrivacyPolicyUrl { get; set; }
        public bool HasPrivacyPolicyUrl => !string.IsNullOrEmpty(PrivacyPolicyUrl);

        public bool HasPrivacyPolicy { get; set; }
        public bool Featured { get; set; }

        public string Headline { get; set; }
        public bool HasHeadline => !string.IsNullOrEmpty(Headline);
        public bool Published { get; set; }
        public bool IsCampaignManager { get; set; }
    }

    public static class CampaignViewModelExtensions
    {
        public static CampaignViewModel ToViewModel(this Models.Campaign campaign)
        {
            return new CampaignViewModel(campaign);
        }

        public static IEnumerable<CampaignViewModel> ToViewModel(this IEnumerable<Models.Campaign> campaigns)
        {
            return campaigns.Select(campaign => campaign.ToViewModel());
        }       
    }
}
