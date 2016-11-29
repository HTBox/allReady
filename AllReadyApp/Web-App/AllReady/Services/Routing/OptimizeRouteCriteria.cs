using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;

namespace AllReady.Services.Routing
{
    public class OptimizeRouteCriteria
    {
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

        public string StartAddress { get; private set; }
        public string EndAddress { get; private set; }
        public List<OptimizeRouteWaypoint> Waypoints { get; private set; }
    }

    public class OptimizeRouteWaypoint
    {
        public OptimizeRouteWaypoint(double longitude, double latitude, Guid requestId)
        {
            Coordinates = string.Join(",", latitude.ToString(), longitude.ToString());
            RequestId = requestId;
        }

        public string Coordinates { get; }

        public Guid RequestId { get; }
    }
}
