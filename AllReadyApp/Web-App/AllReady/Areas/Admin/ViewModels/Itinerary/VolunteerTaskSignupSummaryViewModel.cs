namespace AllReady.Areas.Admin.ViewModels.Itinerary
{
    public class VolunteerTaskSignupSummaryViewModel
    {
        public int VolunteerTaskSignupId { get; set; }

        public string VolunteerName { get; set; }

        public string VolunteerEmail { get; set; }

        public string Title { get; set; }

        public bool UserIsOrgAdmin { get; set; }

        public int ItineraryId { get; set; }
    }
}