﻿namespace AllReady.Areas.Admin.ViewModels.Itinerary
{
    public class TaskSignupSummaryViewModel
    {
        public int TaskSignupId { get; set; }

        public string VolunteerName { get; set; }

        public string VolunteerEmail { get; set; }

        public string Title { get; set; }

        public bool UserIsOrgAdmin { get; set; }

        public int ItineraryId { get; set; }
    }
}