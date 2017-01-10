using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AllReady.Services.Routing.Models.Google;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;

namespace AllReady.Services.Mapping
{
    public class GoogleGeocodeService : IGeocodeService
    {
        private readonly IHttpClient _httpClient;
        private readonly MappingSettings _mappingSettings;

        public GoogleGeocodeService(IHttpClient httpClient, IOptions<MappingSettings> mappingSettings)
        {
            _httpClient = httpClient;
            _mappingSettings = mappingSettings?.Value;
        }

        public async Task<Coordinates> GetCoordinatesFromAddress(string address)
        {
            var requestUrl = GenerateGoogleAPIUrl(address);

            return await ProcessRequest(requestUrl);
        }

        public async Task<Coordinates> GetCoordinatesFromAddress(string address, string city, string state, string postalCode, string country)
        {
            var fullAddress = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(address))
            {
                fullAddress.Append(address).Append(",");
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                fullAddress.Append(city).Append(",");
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                fullAddress.Append(state).Append(",");
            }

            if (!string.IsNullOrWhiteSpace(postalCode))
            {
                fullAddress.Append(postalCode).Append(",");
            }

            if (!string.IsNullOrWhiteSpace(country))
            {
                fullAddress.Append(country).Append(",");
            }

            fullAddress.Remove(fullAddress.Length - 1, 1);

            return await GetCoordinatesFromAddress(fullAddress.ToString());
        }

        private async Task<Coordinates> ProcessRequest(string url)
        {
            try
            {
                // todo sgordon: enhance with Polly for retries
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<GoogleGeocodeResponse>(content);

                    if (result.Status == GoogleDirectionsResponse.OkStatus)
                    {                     
                        var firstResult = result.Results.FirstOrDefault();

                        if (firstResult != null)
                        {
                            var latitude = firstResult.Geometry?.Location?.Latitude;
                            var longitude = firstResult.Geometry?.Location?.Longitude;

                            if (!string.IsNullOrEmpty(latitude) && !string.IsNullOrEmpty(longitude))
                            {
                                var location = new Coordinates(latitude, longitude);

                                return location;
                            }                            
                        }
                    }
                }
            }
            catch
            {
                // todo - logging
            }

            return null;
        }

        private string GenerateGoogleAPIUrl(string address)
        {
            var requestUrl = new StringBuilder("https://maps.googleapis.com/maps/api/geocode/json?address=");
            requestUrl.Append(UrlEncoder.Default.Encode(address));

            requestUrl.Append("&key=").Append(_mappingSettings.GoogleDirectionsApiKey);

            return requestUrl.ToString();
        }
    }
}
