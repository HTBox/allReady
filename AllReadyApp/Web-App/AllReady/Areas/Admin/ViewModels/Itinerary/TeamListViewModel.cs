namespace AllReady.Areas.Admin.ViewModels.Itinerary
{
    public class TeamListViewModel
    {
        public int TaskSignupId { get; set; }
        public string VolunteerEmail { get; set; }
        public string TaskName { get; set; }
        public string FullName { get; set; }

        public string DisplayName => !string.IsNullOrEmpty(FullName) ? FullName : "Not available";
    }
}
