using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllReady.Services.Routing.Models.Google;

namespace AllReady.Services.Routing
{
    public class GoogleOptimizeRouteService : IOptimizeRouteService
    {
        private readonly MappingSettings _mappingSettings;
        private readonly ILogger<GoogleOptimizeRouteService> _logger;
        private static IHttpClient _httpClient;

        public GoogleOptimizeRouteService(IOptions<MappingSettings> mappingSettings, ILogger<GoogleOptimizeRouteService> logger, IHttpClient httpClient)
        {
            _mappingSettings = mappingSettings?.Value;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<OptimizeRouteResult> OptimizeRoute(OptimizeRouteCriteria criteria)
        {
            var requestIds = new List<Guid>();

            if (!string.IsNullOrEmpty(_mappingSettings?.GoogleMapsApiKey))
            {
                try
                {
                    // todo sgordon: enhance with Polly for retries
                    var response = await _httpClient.GetAsync(GenerateGoogleAPIUrl(criteria));

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<GoogleDirectionsResponse>(content);

                        if (result.Status == GoogleDirectionsResponse.OkStatus)
                        {
                            requestIds.AddRange(result.Routes.SelectMany(r => r.WaypointOrder).Select(waypointIndex => criteria.Waypoints[waypointIndex].RequestId));

                            return new OptimizeRouteResult { RequestIds = requestIds, Distance = result.TotalDistance, Duration = result.TotalDuration };
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failure to contact Google Directions API - {ex}");
                }
            }

            return null;
        }

        private string GenerateGoogleAPIUrl(OptimizeRouteCriteria criteria)
        {
            var requestUrl = new StringBuilder("https://maps.googleapis.com/maps/api/directions/json?origin=");
            requestUrl.Append(criteria.StartAddress);
            requestUrl.Append("&destination=").Append(criteria.EndAddress);
            requestUrl.Append("&waypoints=optimize:true");

            foreach (var waypoint in criteria.Waypoints)
            {
                requestUrl.Append("|").Append(waypoint.Coordinates);
            }

            requestUrl.Append("&key=").Append(_mappingSettings.GoogleMapsApiKey);

            return requestUrl.ToString();
        }
    }
}
