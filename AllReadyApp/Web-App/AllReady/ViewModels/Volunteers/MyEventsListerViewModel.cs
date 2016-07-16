using System.Collections.Generic;

namespace AllReady.ViewModels
{
    public class MyEventsListerViewModel
    {
        public IEnumerable<MyEventsListerItem> CurrentEvents { get; set; } = new List<MyEventsListerItem>();
        public IEnumerable<MyEventsListerItem> FutureEvents { get; set; } = new List<MyEventsListerItem>();
        public IEnumerable<MyEventsListerItem> PastEvents { get; set; } = new List<MyEventsListerItem>();
    }
}
