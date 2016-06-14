using System.Collections.Generic;
using System.Linq;
using AllReady.ViewModels.Task;

namespace AllReady.ViewModels
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
