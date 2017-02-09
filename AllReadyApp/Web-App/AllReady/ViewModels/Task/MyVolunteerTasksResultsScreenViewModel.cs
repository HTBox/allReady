using System.Collections.Generic;
using System.Linq;

namespace AllReady.ViewModels.Task
{
    public class MyVolunteerTasksResultsScreenViewModel
    {
        public MyVolunteerTasksResultsScreenViewModel(string title, IEnumerable<VolunteerTaskViewModel> items)
        {
            Title = title;
            Items = items.ToList();
        }

        public string Title { get; set; }

        public List<VolunteerTaskViewModel> Items { get; set; }
    }
}
