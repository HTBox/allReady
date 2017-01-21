using System.Text;

namespace AllReady.Caching
{
    /// <summary>
    /// Builds cache keys for allReady
    /// </summary>
    public static class CacheKeyBuilder
    {
        /// <summary>
        /// Prefix of the cache key used for optimize route results
        /// </summary>
        public static string OptimizeRouteResultCacheKeyPrefix = "optimize-route";

        /// <summary>
        /// Builds a cache key for <see cref="OptimizeRouteResultStatus"/>
        /// </summary>
        public static string BuildOptimizeRouteCacheKey(string userId, int itineraryId)
        {
            var cacheKey = new StringBuilder(OptimizeRouteResultCacheKeyPrefix).Append("-");
            cacheKey.Append(userId).Append("-");
            cacheKey.Append(itineraryId);

            return cacheKey.ToString();
        }
    }
}
