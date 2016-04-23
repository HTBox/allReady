using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Models;

namespace AllReady.ViewModels
{
    public class CampaignViewModel
    {
        public CampaignViewModel()
        {
            Events = new List<EventViewModel>();
        }

        public CampaignViewModel(Campaign campaign)
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
            TimeZoneId = campaign.TimeZoneId;
            StartDate = campaign.StartDateTime;
            EndDate = campaign.EndDateTime;
            Events = campaign.Events != null ? campaign.Events.ToViewModel() : Enumerable.Empty<EventViewModel>();
            CampaignImpact = campaign.CampaignImpact;
            ImageUrl = campaign.ImageUrl;
            HasPrivacyPolicy = !string.IsNullOrEmpty(campaign.ManagingOrganization?.PrivacyPolicy);
            Location = campaign.Location;
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

        public CampaignImpact CampaignImpact { get; set; }

        public List<CampaignSponsors> ParticipatingOrganizations { get; set; }

        public string TimeZoneId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public IEnumerable<EventViewModel> Events { get; set; }

        public bool HasPrivacyPolicy { get; set; }
    }

    public static class CampaignViewModelExtensions
    {
        public static CampaignViewModel ToViewModel(this Campaign campaign)
        {
            return new CampaignViewModel(campaign);
        }

        public static IEnumerable<CampaignViewModel> ToViewModel(this IEnumerable<Campaign> campaigns)
        {
            return campaigns.Select(campaign => campaign.ToViewModel());
        }       
    }
}
