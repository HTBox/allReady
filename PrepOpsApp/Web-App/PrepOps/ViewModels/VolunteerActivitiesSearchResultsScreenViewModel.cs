using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrepOps.ViewModels
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
