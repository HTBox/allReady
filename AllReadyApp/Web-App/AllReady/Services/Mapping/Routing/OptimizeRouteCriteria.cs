using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;

namespace AllReady.Services.Mapping.Routing
{
    /// <summary>
    /// Defines the properties required as input for route optimization
    /// </summary>
    public class OptimizeRouteCriteria
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OptimizeRouteCriteria"/>
        /// </summary>
        public OptimizeRouteCriteria(string startAddress, string endAddress, List<OptimizeRouteWaypoint> waypoints)
        {
            if (string.IsNullOrWhiteSpace(startAddress))
            {
                throw new ArgumentException(nameof(startAddress));
            }

            if (string.IsNullOrWhiteSpace(endAddress))
            {
                throw new ArgumentException(nameof(endAddress));
            }

            if (waypoints == null || !waypoints.Any())
            {
                throw new ArgumentException(nameof(waypoints));
            }

            // Ensure that all string values are URL encoded            
            StartAddress = UrlEncoder.Default.Encode(startAddress);
            EndAddress = UrlEncoder.Default.Encode(endAddress);

            Waypoints = waypoints;
        }

        /// <summary>
        /// The start address for the optmized route
        /// </summary>
        public string StartAddress { get; private set; }

        /// <summary>
        /// The end address for the optmized route
        /// </summary>
        public string EndAddress { get; private set; }

        /// <summary>
        /// A list of waypoints to be optimized for the route
        /// </summary>
        public List<OptimizeRouteWaypoint> Waypoints { get; private set; }
    }
}
