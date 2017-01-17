using AllReady.Caching;
using AllReady.Services.Mapping.Routing;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class OptimizeRouteResultFromCacheQueryHandler : IRequestHandler<OptimizeRouteResultFromCacheQuery, OptimizeRouteResultStatus>
    {
        private readonly IMemoryCache _cache;

        public OptimizeRouteResultFromCacheQueryHandler(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public OptimizeRouteResultStatus Handle(OptimizeRouteResultFromCacheQuery message)
        {
            OptimizeRouteResultStatus optimizeResultMessage = null;

            if (_cache.TryGetValue(string.Concat(CacheKeys.OptimizeRouteResultCache, message.UserId, message.ItineraryId), out optimizeResultMessage))
            {
                _cache.Remove(string.Concat(CacheKeys.OptimizeRouteResultCache, message.UserId, message.ItineraryId));
            }

            return optimizeResultMessage;
        }
    }
}
