﻿using System.Collections.Generic;
using AllReady.ViewModels.Event;

namespace AllReady.ViewModels
{
    public class VolunteeEventsSearchResultsScreenViewModel
    {
        public VolunteeEventsSearchResultsScreenViewModel()
        {
            this.Events = new List<EventViewModel>();
        }

        public string Title { get; set; }
        public string GoButtonLabel { get; set; }
        public List<EventViewModel> Events { get; set; }
    }
}
