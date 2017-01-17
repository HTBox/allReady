using AllReady.Services.Mapping.Routing;
using MediatR;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class OptimizeRouteResultFromCacheQuery : IRequest<OptimizeRouteResultStatus>
    {
        public string UserId { get; set; }
        public int ItineraryId { get; set; }
    }
}
