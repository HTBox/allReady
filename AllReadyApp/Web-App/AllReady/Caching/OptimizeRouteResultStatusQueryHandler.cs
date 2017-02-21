using AllReady.Caching;
using AllReady.Services.Mapping.Routing;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace AllReady.Caching
{
    public class OptimizeRouteResultStatusQueryHandler : IRequestHandler<OptimizeRouteResultStatusQuery, OptimizeRouteResultStatus>
    {
        private readonly IMemoryCache _cache;

        public OptimizeRouteResultStatusQueryHandler(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public OptimizeRouteResultStatus Handle(OptimizeRouteResultStatusQuery message)
        {
            OptimizeRouteResultStatus optimizeResultMessage;

            var cacheKey = CacheKeyBuilder.BuildOptimizeRouteCacheKey(message.UserId, message.ItineraryId);

            if (_cache.TryGetValue(cacheKey, out optimizeResultMessage))
            {
                _cache.Remove(cacheKey);
            }

            return optimizeResultMessage;
        }
    }
}
