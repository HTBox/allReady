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

        public List<TeamListModel> TeamMembers { get; set; } = new List<TeamListModel>();

        public string DisplayDate => Date.ToLongDateString();
    }
}
