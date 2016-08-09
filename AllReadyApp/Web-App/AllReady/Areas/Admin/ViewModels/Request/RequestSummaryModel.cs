using AllReady.Areas.Admin.Models.ItineraryModels;

namespace AllReady.Areas.Admin.Models.RequestModels
{
    public class RequestSummaryModel : RequestListModel
    {
        public string State { get; set; }
        public string Zip { get; set; }
    }
}