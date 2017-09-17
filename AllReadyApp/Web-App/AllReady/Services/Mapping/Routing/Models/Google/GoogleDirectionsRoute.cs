using System.Collections.Generic;
using Newtonsoft.Json;

namespace AllReady.Services.Mapping.Routing.Models.Google
{
    public class GoogleDirectionsRoute
    {
        [JsonProperty(PropertyName = "waypoint_order")]
        public List<int> WaypointOrder { get; set; } = new List<int>();
        public List<GoogleDirectionsRouteLeg> Legs { get; set; } = new List<GoogleDirectionsRouteLeg>();
    }
}
