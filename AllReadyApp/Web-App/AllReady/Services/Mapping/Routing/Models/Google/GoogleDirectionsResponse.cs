using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AllReady.Services.Mapping.Routing.Models.Google
{
    public class GoogleDirectionsResponse
    {
        public static string OkStatus = "OK";
        public static string NoResultsStatus = "ZERO_RESULTS";
        public static string NotFound = "NOT_FOUND";

        public string Status { get; set; }
        public List<GoogleDirectionsRoute> Routes { get; set; } = new List<GoogleDirectionsRoute>();

        [JsonIgnore]
        public int TotalDuration => Routes.SelectMany(x => x.Legs).Sum(x => x.Duration.Value);
        public int TotalDistance => Routes.SelectMany(x => x.Legs).Sum(x => x.Distance.Value);
    }
}
