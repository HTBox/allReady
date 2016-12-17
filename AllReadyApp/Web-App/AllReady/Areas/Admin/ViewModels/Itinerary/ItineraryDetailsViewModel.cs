using System;
using System.Collections.Generic;
using AllReady.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AllReady.Areas.Admin.ViewModels.Itinerary
{
    /// <summary>
    /// Defines data used by the admin itinerary details page
    /// </summary>
    public class ItineraryDetailsViewModel
    {
        /// <summary>
        /// The ID of the itinerary being displayed
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the itinerary being displayed
        /// </summary>
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int OrganizationId { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }

        public int SelectedTeamMember { get; set; }
        public IEnumerable<SelectListItem> PotentialTeamMembers { get; set; } = new List<SelectListItem>();
        public bool HasPotentialTeamMembers { get; set; }

        public List<TeamListViewModel> TeamMembers { get; set; } = new List<TeamListViewModel>();
        public List<RequestListViewModel> Requests { get; set; } = new List<RequestListViewModel>();

        public string DisplayDate => Date.ToLongDateString();

        /// <summary>
        /// The display address for the itinerary start location
        /// </summary>
        public string StartAddress { get; set; }

        /// <summary>
        /// The display address for the itinerary end location
        /// </summary>
        public string EndAddress { get; set; }

        /// <summary>
        /// Indicates whether the itinerary contains the required start/end address combination in order to enable optimize and view route functions 
        /// </summary>
        public bool CanOptimizeAndDisplayRoute { get; set; }

        /// <summary>
        /// Indicates that the start and end address are the same
        /// </summary>
        public bool UseStartAddressAsEndAddress { get; set; }

        /// <summary>
        /// A URL to the bing maps site with parameters needed to generate a driving route
        /// </summary>
        public string BingMapUrl { get; set; }

        public string RequestKeywords { get; set; }

        public RequestStatus? RequestStatus { get; set; }
    }
}