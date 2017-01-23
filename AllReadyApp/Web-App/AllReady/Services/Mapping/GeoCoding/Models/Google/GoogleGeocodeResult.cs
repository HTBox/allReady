namespace AllReady.Services.Mapping.GeoCoding.Models.Google
{
    /// <summary>
    /// Maps properties from a Google Geocode API call JSON response
    /// </summary>
    public class GoogleGeocodeResult
    {
        public GoogleGeocodeResultGeometry Geometry { get; set; }
    }
}
