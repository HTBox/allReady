using AllReady.Areas.Admin.ViewModels.Itinerary;
using MediatR;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class ItineraryDetailQuery : IAsyncRequest<ItineraryDetailsViewModel>
    {
        public int ItineraryId { get; set; }
    }
}
