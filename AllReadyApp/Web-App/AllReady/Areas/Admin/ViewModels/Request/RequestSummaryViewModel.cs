﻿using AllReady.Areas.Admin.ViewModels.Itinerary;

namespace AllReady.Areas.Admin.ViewModels.Request
{
    public class RequestSummaryViewModel : RequestListViewModel
    {
        public string State { get; set; }
        public string Zip { get; set; }
    }
}