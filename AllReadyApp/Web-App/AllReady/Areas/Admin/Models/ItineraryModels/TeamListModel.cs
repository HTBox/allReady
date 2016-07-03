namespace AllReady.Areas.Admin.Models.ItineraryModels
{
    public class TeamListModel
    {
        public int TaskSignupId { get; set; }
        public string VolunteerEmail { get; set; }
        public string TaskName { get; set; }
        public string FullName { get; set; }

        public string DisplayName => !string.IsNullOrEmpty(FullName) ? FullName : "Not available";
    }
}
