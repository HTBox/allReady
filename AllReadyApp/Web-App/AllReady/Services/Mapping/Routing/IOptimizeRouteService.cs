using System.Threading.Tasks;

namespace AllReady.Services.Mapping.Routing
{
    /// <summary>
    /// A service which provides route optimization
    /// </summary>
    public interface IOptimizeRouteService
    {
        /// <summary>
        /// Optimize a set of route criteria / waypoints
        /// </summary>
        Task<OptimizeRouteResult> OptimizeRoute(OptimizeRouteCriteria criteria);
    }
}