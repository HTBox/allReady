using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Itinerary;

namespace AllReady.Areas.Admin.ViewModels.Request
{
    public class UnlinkedRequestsViewModel
    {
        public List<RequestListViewModel> Requests { get; set; } = new List<RequestListViewModel>();
    }
}
