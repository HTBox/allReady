using AllReady.Areas.Admin.Models.ItineraryModels;
using MediatR;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class EditItineraryCommand : IAsyncRequest<int>
    {
        public ItineraryEditModel Itinerary { get; set; }
    }
}
