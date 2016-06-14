using System.Collections.Generic;
using System.Linq;
using AllReady.ViewModels.Shared;

namespace AllReady.ViewModels
{
    public class MyEventsResultsScreenViewModel
    {
        public MyEventsResultsScreenViewModel(string title, IList<EventViewModel> items)
        {
            Title = title;
            Items = items;
        }

        public string Title { get; set; }

        public IList<EventViewModel> Items { get; set; }
    }
}
