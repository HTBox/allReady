using System;
using System.Collections.Generic;

namespace AllReady.Services.Mapping.Routing
{
    /// <summary>
    /// Result of route optimization
    /// </summary>
    public class OptimizeRouteResult
    {
        /// <summary>
        /// A list of request ids in optimized order
        /// </summary>
        public List<Guid> RequestIds { get; set; }

        /// <summary>
        /// The total distance in meters of the route
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        /// The total duration in minutes of the route
        /// </summary>
        public int Duration { get; set; }
    }
}
