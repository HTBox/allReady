using AllReady.Areas.Admin.ViewModels.Itinerary;
using MediatR;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class EditItineraryCommand : IAsyncRequest<int>
    {
        public ItineraryEditViewModel Itinerary { get; set; }
    }
}
