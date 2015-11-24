using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllReady.ViewModels
{
    public class CampaignViewModel
    {
        public CampaignViewModel()
        {
            Activities = new List<ActivityViewModel>();
        }

        public CampaignViewModel(Campaign campaign)
        {
            if (campaign == null)
                return;

            Id = campaign.Id;
            Name = campaign.Name;
            Description = campaign.Description;
            FullDescription = campaign.FullDescription;
            ///TODO: Commented out as campaign.ManagingTenant is null from sample data;
            /// Fix sample provider to ensure that property is not null
            //ManagingTenantName = campaign.ManagingTenant.Name;
            //ManagingTenantId = campaign.ManagingTenant.Id;
            TimeZoneId = campaign.TimeZoneId;
            StartDate = campaign.StartDateTime;
            EndDate = campaign.EndDateTime;
            Activities = campaign.Activities != null ? campaign.Activities.ToViewModel() : Enumerable.Empty<ActivityViewModel>();
            CampaignImpact = campaign.CampaignImpact;
            ImageUrl = campaign.ImageUrl;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string FullDescription { get; set; }

        public string ImageUrl { get; set; }

        public int ManagingTenantId { get; set; }

        public string ManagingTenantName { get; set; }

        public CampaignImpact CampaignImpact { get; set; }

        public List<CampaignSponsors> ParticipatingTenants { get; set; }

        public string TimeZoneId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public IEnumerable<ActivityViewModel> Activities { get; set; }
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
