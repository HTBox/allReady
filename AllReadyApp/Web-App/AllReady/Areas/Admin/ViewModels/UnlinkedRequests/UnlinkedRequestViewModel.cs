using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AllReady.Areas.Admin.ViewModels.UnlinkedRequests
{
    public class UnlinkedRequestViewModel
    {   
        public List<RequestSelectViewModel> Requests { get; set; } = new List<RequestSelectViewModel>();
        public List<SelectListItem> Events { get; set; } = new List<SelectListItem>();
        public int EventId { get; set; }
    }
}
