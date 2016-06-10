using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace AllReady.Areas.Admin.Models.ItineraryModels
{
    public class ItineraryDetailsModel
    {
        public int Id { get; set; }
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

        public List<TeamListModel> TeamMembers { get; set; } = new List<TeamListModel>();
        public List<RequestListModel> Requests { get; set; } = new List<RequestListModel>();

        public string DisplayDate => Date.ToLongDateString();
    }
}
