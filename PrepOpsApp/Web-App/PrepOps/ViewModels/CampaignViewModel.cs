using PrepOps.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrepOps.ViewModels
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
            ManagingTenantName = campaign.ManagingTenant.Name;
            ManagingTenantId = campaign.ManagingTenant.Id;
            StartDate = new DateTimeOffset(campaign.StartDateTimeUtc, TimeSpan.Zero);
            EndDate = new DateTimeOffset(campaign.EndDateTimeUtc, TimeSpan.Zero);
            Activities = campaign.Activities != null ? campaign.Activities.ToViewModel() : Enumerable.Empty<ActivityViewModel>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int ManagingTenantId { get; set; }

        public string ManagingTenantName { get; set; }

        public List<CampaignSponsors> ParticipatingTenants { get; set; }

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

        public static Campaign ToModel(this CampaignViewModel campaign, IPrepOpsDataAccess dataAccess)
        {
            var tenant = dataAccess.GetTenant(campaign.ManagingTenantId);

            if (tenant == null)
                return null;

            return new Campaign
            {
                Id = campaign.Id,
                Description = campaign.Description,
                Name = campaign.Name,
                ManagingTenant = tenant,
                ParticipatingTenants = campaign.ParticipatingTenants,
                Activities = campaign.Activities.ToModel(dataAccess).ToList(),
                EndDateTimeUtc = campaign.EndDate.UtcDateTime,
                StartDateTimeUtc = campaign.StartDate.UtcDateTime
            };
        }

        public static IEnumerable<Campaign> ToModel(this IEnumerable<CampaignViewModel> campaigns, IPrepOpsDataAccess dataAccess)
        {
            return campaigns.Select(campaign => campaign.ToModel(dataAccess));
        }

    }
}
