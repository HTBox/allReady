using System.Collections.Generic;

namespace AllReady.ViewModels
{
    public class VolunteerActivitiesSearchResultsScreenViewModel
    {
        public VolunteerActivitiesSearchResultsScreenViewModel()
        {
            this.Activities = new List<ActivityViewModel>();
        }

        public string Title { get; set; }
        public string GoButtonLabel { get; set; }
        public List<ActivityViewModel> Activities { get; set; }
    }
}
