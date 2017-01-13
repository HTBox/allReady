using Newtonsoft.Json;

namespace AllReady.Services.Mapping.GeoCoding.Models.Google
{
    /// <summary>
    /// Maps properties from a Google Geocode API call JSON response
    /// </summary>
    public class GoogleGeocodeResultLocation
    {
        [JsonProperty("lat")]
        public string Latitude { get; set; }

        [JsonProperty("lng")]
        public string Longitude { get; set; }
    }
}
