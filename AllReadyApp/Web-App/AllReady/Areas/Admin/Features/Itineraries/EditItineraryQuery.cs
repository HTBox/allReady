using AllReady.Areas.Admin.ViewModels.Itinerary;
using MediatR;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class EditItineraryQuery : IAsyncRequest<ItineraryEditViewModel>
    {
        public int ItineraryId { get; set; }
    }
}
