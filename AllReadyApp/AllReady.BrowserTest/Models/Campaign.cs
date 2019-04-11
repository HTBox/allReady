using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Models
{
    public class Campaign
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Headline { get; set; }
        public string FullDesciption { get; set; }
        public string TimeZone { get; set; }
        public string StartDate { get; set; }
        public string Organization { get; set; }
        public bool Published { get; set; }
    }
}
