using System;
using System.Collections.Generic;

namespace AllReady.Services.Mapping.Routing
{
    /// <summary>
    /// Result of route optimization
    /// </summary>
    public class OptimizeRouteResult
    {
        public static OptimizeRouteResult FailedOptimizeRouteResult(string failureMessage)
        {
            if (string.IsNullOrEmpty(failureMessage))
                throw new ArgumentNullException(nameof(failureMessage));

            return new OptimizeRouteResult
            {
                Status = new OptimizeRouteResultStatus
                {
                    IsSuccess = false,
                    StatusMessage = failureMessage
                }
            };
        }

        /// <summary>
        /// A list of request ids in optimized order
        /// </summary>
        public List<Guid> RequestIds { get; set; } = new List<Guid>();

        /// <summary>
        /// The total distance in meters of the route
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        /// The total duration in minutes of the route
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Indicates the status of the optimize route process
        /// </summary>
        public OptimizeRouteResultStatus Status { get; set; } = new OptimizeRouteResultStatus();
    }
}
