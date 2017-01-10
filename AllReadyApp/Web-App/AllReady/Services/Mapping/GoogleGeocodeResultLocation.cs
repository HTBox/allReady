using Newtonsoft.Json;

namespace AllReady.Services.Mapping
{
    public class GoogleGeocodeResultLocation
    {
        [JsonProperty("lat")]
        public string Latitude { get; set; }

        [JsonProperty("lng")]
        public string Longitude { get; set; }
    }
}
