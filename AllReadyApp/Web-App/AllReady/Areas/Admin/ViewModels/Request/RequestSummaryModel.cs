using AllReady.Areas.Admin.ViewModels.Itinerary;

namespace AllReady.Areas.Admin.ViewModels.Request
{
    public class RequestSummaryModel : RequestListModel
    {
        public string State { get; set; }
        public string Zip { get; set; }
    }
}