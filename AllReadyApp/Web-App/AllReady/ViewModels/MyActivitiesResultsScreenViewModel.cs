using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.ViewModels
{
    public class MyActivitiesResultsScreenViewModel
    {
        public MyActivitiesResultsScreenViewModel(string title, IEnumerable<ActivityViewModel> items)
        {
            Title = title;
            Items = items.ToList();
        }

        public string Title { get; set; }

        public List<ActivityViewModel> Items { get; set; }
    }
}
