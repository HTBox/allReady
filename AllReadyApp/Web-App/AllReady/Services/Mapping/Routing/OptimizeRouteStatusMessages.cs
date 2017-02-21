
namespace AllReady.Services.Mapping.Routing
{
    /// <summary>
    /// Single place to define the wording of Optimize Route Statuses
    /// </summary>
    public static class OptimizeRouteStatusMessages
    {
        /// <summary>
        /// A general failure message when a specific cause cannot be / should not be provided to the end user
        /// </summary>
        public static string GeneralOptimizeFailure = "Unable to optimize route";

        /// <summary>
        /// Indicates that the optimization process completed, but that the order of requests did not change
        /// </summary>
        public static string AlreadyOptimized = "Route already most optimal";

        /// <summary>
        /// Indicates that the optimization process completed and the request order was updated
        /// </summary>
        public static string OptimizeSucess = "Route optimized";

        /// <summary>
        /// Indicates that the optimization process completed but no waypoints where included
        /// </summary>
        public static string ZeroResults = "Unable to find one or more of the request addresses";
    }
}
