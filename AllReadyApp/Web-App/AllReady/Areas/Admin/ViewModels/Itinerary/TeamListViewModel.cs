namespace AllReady.Areas.Admin.ViewModels.Itinerary
{
    public class TeamListViewModel
    {
        public int VolunteerTaskSignupId { get; set; }
        public string VolunteerEmail { get; set; }
        public string VolunteerTaskName { get; set; }
        public string FullName { get; set; }

        /// <summary>
        /// Indicates that this team member is the designated team lead
        /// </summary>
        public bool IsTeamLead { get; set; }

        public string DisplayName => !string.IsNullOrWhiteSpace(FullName) ? FullName + (IsTeamLead ? " (Team Lead)" : "") : "Not available" + (IsTeamLead ? " (Team Lead)" : "");
    }
}
