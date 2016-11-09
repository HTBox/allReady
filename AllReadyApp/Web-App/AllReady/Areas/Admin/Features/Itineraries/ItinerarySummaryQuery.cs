using AllReady.Areas.Admin.ViewModels.Itinerary;
using MediatR;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class ItinerarySummaryQuery : IAsyncRequest<ItinerarySummaryViewModel>
    {
        public int ItineraryId { get; set; }
    }
}
