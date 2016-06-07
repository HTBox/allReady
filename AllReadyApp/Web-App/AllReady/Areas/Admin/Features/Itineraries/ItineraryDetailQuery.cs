using AllReady.Areas.Admin.Models.ItineraryModels;
using MediatR;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class ItineraryDetailQuery : IAsyncRequest<ItineraryDetailsModel>
    {
        public int ItineraryId { get; set; }
    }
}
