using System;
using System.Collections.Generic;
using AllReady.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using AllReady.Services.Mapping.Routing;
using System.Linq;

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

        /// <summary>
        /// Holds a list of the values for the team lead select list
        /// </summary>
        public IEnumerable<SelectListItem> PotentialTeamLeads { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Indicates that this itinerary has alternative team leads available
        /// </summary>
        public bool HasPotentialTeamLeads => PotentialTeamLeads.Any();

        /// <summary>
        /// Indicates that this itinerary has a team lead assigned
        /// </summary>
        public bool HasTeamLead => TeamMembers.Any(t => t.IsTeamLead);

        public List<RequestListViewModel> Requests { get; set; } = new List<RequestListViewModel>();

        /// <summary>
        /// Set true if the itinerary has any requests before filtering
        /// </summary>
        public bool HasAnyRequests { get; set; }

        public string DisplayDate => Date.ToString("D");

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

        /// <summary>
        /// Only set if an optimize route result is found in the cache for the current user
        /// </summary>
        public OptimizeRouteResultStatus OptimizeRouteStatus { get; set; }

        public bool? TeamLeadChangedSuccess { get; set; }

        public bool HasTeamLeadResult => TeamLeadChangedSuccess.HasValue;

        /// <summary>
        /// Identifies whether team management functions are enabled and should be shown in the UI.
        /// </summary>
        public bool TeamManagementEnabled { get; set; }
    }
}