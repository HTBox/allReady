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

            var cacheKey = CacheKeyBuilder.BuildOptimizeRouteCacheKey(message.UserId, message.ItineraryId);

            if (_cache.TryGetValue(cacheKey, out optimizeResultMessage))
            {
                _cache.Remove(cacheKey);
            }

            return optimizeResultMessage;
        }
    }
}
