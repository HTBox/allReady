using System.Threading.Tasks;

namespace AllReady.Services.Routing
{
    public interface IOptimizeRouteService
    {
        Task<OptimizeRouteResult> OptimizeRoute(OptimizeRouteCriteria criteria);
    }
}