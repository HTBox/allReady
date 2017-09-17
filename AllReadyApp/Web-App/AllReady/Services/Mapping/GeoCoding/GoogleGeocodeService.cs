using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllReady.Configuration;
using AllReady.Services.Mapping.GeoCoding.Models;
using AllReady.Services.Mapping.GeoCoding.Models.Google;
using AllReady.Services.Mapping.Routing.Models.Google;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AllReady.Services.Mapping.GeoCoding
{
    /// <summary>
    /// An implementation of the Geocode Service which uses the Google API
    /// </summary>
    public class GoogleGeocodeService : IGeocodeService
    {
        private readonly IHttpClient _httpClient;
        private readonly MappingSettings _mappingSettings;

        /// <summary>
        /// Initialises a new instance of the <see cref="GoogleGeocodeService"/>
        /// </summary>
        public GoogleGeocodeService(IHttpClient httpClient, IOptions<MappingSettings> mappingSettings)
        {
            _httpClient = httpClient;
            _mappingSettings = mappingSettings?.Value;
        }

        /// <inheritdoc />
        public async Task<Coordinates> GetCoordinatesFromAddress(string address)
        {
            var requestUrl = GenerateGoogleApiUrl(address);

            return await ProcessRequest(requestUrl);
        }

        /// <inheritdoc />
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

            if (fullAddress.Length > 0)
            { 
                fullAddress.Remove(fullAddress.Length - 1, 1);
            }

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

        private string GenerateGoogleApiUrl(string address)
        {
            var requestUrl = new StringBuilder("https://maps.googleapis.com/maps/api/geocode/json?address=");
            requestUrl.Append(Uri.EscapeUriString(address));

            requestUrl.Append("&key=").Append(_mappingSettings.GoogleMapsApiKey);

            return requestUrl.ToString();
        }
    }
}
