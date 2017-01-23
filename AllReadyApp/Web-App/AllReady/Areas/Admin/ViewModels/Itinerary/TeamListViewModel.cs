namespace AllReady.Areas.Admin.ViewModels.Itinerary
{
    public class TeamListViewModel
    {
        public int VolunteerTaskSignupId { get; set; }
        public string VolunteerEmail { get; set; }
        public string VolunteerTaskName { get; set; }
        public string FullName { get; set; }

        public string DisplayName => !string.IsNullOrEmpty(FullName) ? FullName : "Not available";
    }
}
