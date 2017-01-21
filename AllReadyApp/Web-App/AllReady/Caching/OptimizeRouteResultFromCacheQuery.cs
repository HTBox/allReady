using AllReady.Services.Mapping.Routing;
using MediatR;

namespace AllReady.Caching
{
    public class OptimizeRouteResultFromCacheQuery : IRequest<OptimizeRouteResultStatus>
    {
        public string UserId { get; set; }
        public int ItineraryId { get; set; }
    }
}
