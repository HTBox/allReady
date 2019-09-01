using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest.Models
{
    public class VolunteerTask
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfVolunteersRequired { get; set; }
        public string StartDateTime { get; set; }
    }
}
