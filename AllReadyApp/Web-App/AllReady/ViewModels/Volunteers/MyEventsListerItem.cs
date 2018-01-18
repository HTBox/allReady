using System;
using System.Collections.Generic;

namespace AllReady.ViewModels
{
    public class MyEventsListerItem
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string TimeZone { get; set; }
        public string Campaign { get; set; }
        public string Organization { get; set; }
        public int VolunteerCount { get; set; }

        public List<MyEventsListerItemTask> Tasks { get; set; } = new List<MyEventsListerItemTask>();
    }
}
