using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels.Event;

namespace AllReady.ViewModels.Campaign
{
    public class ManageCampaignViewModel
    {
        public ManageCampaignViewModel()
        {
           
        }

        public ManageCampaignViewModel(Models.Campaign campaign)
        {
            if (campaign == null)
                return;

            Id = campaign.Id;
            Name = campaign.Name;
            Description = campaign.Description;
            FullDescription = campaign.FullDescription;
            ExternalUrl = campaign.ExternalUrl;
            ExternalUrlText = campaign.ExternalUrlText;
            TimeZoneId = campaign.TimeZoneId;
            StartDate = campaign.StartDateTime;
            EndDate = campaign.EndDateTime;            
            ImageUrl = campaign.ImageUrl;
            Published = campaign.Published;
            Featured = campaign.Featured;

        }


        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ExternalUrl { get; set; }
        
        public string ExternalUrlText { get; set; }

        public string FullDescription { get; set; }

        public string ImageUrl { get; set; }

        public string TimeZoneId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public bool IsCampaignManager { get; set; }

        public bool Featured { get; set; }

        public bool Published { get; set; }
    }

    public static class ManageCampaignViewModelExtensions
    {
        public static ManageCampaignViewModel ToManageCampaignViewModel(this Models.Campaign campaign)
        {
            return new ManageCampaignViewModel(campaign);
        }

        public static IEnumerable<ManageCampaignViewModel> ToManageCampaignViewModel(this IEnumerable<Models.Campaign> campaigns)
        {
            return campaigns.Select(campaign => campaign.ToManageCampaignViewModel());
        }       
    }
}