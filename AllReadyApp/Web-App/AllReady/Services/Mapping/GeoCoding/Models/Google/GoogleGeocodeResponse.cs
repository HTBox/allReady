using System.Collections.Generic;

namespace AllReady.Services.Mapping.GeoCoding.Models.Google
{
    /// <summary>
    /// Maps properties from a Google Geocode API call JSON response
    /// </summary>
    public class GoogleGeocodeResponse
    {
        /// <summary>
        /// Defines the default Google OK status value
        /// </summary>
        public static string OkStatus = "OK";

        /// <summary>
        /// The status value of the API call
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// A list of the <see cref="GoogleGeocodeResult"/>s returned by the API
        /// </summary>
        public List<GoogleGeocodeResult> Results { get; set; }
    }
}
