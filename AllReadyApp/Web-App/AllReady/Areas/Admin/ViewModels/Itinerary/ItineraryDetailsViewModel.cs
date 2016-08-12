using System;
using System.Collections.Generic;
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
    }
}
