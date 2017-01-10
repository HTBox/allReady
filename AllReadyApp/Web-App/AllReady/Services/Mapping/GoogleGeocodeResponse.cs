using System.Collections.Generic;

namespace AllReady.Services.Mapping
{
    public class GoogleGeocodeResponse
    {
        public static string OkStatus = "OK";

        public string Status { get; set; }

        public List<GoogleGeocodeResult> Results { get; set; }
    }
}
