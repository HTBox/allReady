using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Models
{
    public class Event
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Headline { get; set; }
        public string EventType { get; set; }
        public bool IsLimitVolunteers { get; set; }
        public string TimeZone { get; set; }
        public string StartDateTime { get; set; }
    }
}
