using System.Collections.Generic;

namespace AllReady.Areas.Admin.ViewModels.Event
{
    public class EventListerViewModel
    {
        public string Organization { get; set; }

        public List<EventListItemViewModel> Events { get; set; } = new List<EventListItemViewModel>();               
    }
}