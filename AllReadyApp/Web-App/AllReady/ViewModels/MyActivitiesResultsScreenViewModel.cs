using System.Collections.Generic;
using System.Linq;

namespace AllReady.ViewModels
{
    public class MyActivitiesResultsScreenViewModel
    {
        public MyActivitiesResultsScreenViewModel(string title, IList<ActivityViewModel> items)
        {
            Title = title;
            Items = items;
        }

        public string Title { get; set; }

        public IList<ActivityViewModel> Items { get; set; }
    }
}
