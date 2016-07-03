using System.Collections.Generic;
using System.Linq;

namespace AllReady.ViewModels.Task
{
    public class MyTasksResultsScreenViewModel
    {
        public MyTasksResultsScreenViewModel(string title, IEnumerable<TaskViewModel> items)
        {
            Title = title;
            Items = items.ToList();
        }

        public string Title { get; set; }

        public List<TaskViewModel> Items { get; set; }
    }
}
