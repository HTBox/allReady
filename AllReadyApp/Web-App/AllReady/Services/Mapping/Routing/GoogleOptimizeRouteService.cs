using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllReady.Services.Mapping.Routing.Models.Google;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;

namespace AllReady.Services.Mapping.Routing
{
    /// <summary>
    /// An implementation of the <see cref="IOptimizeRouteService"/> which uses the Google API
    /// </summary>
    public class GoogleOptimizeRouteService : IOptimizeRouteService
    {
        private readonly MappingSettings _mappingSettings;
        private readonly ILogger<GoogleOptimizeRouteService> _logger;
        private static IHttpClient _httpClient;

        /// <inheritdoc />
        public GoogleOptimizeRouteService(IOptions<MappingSettings> mappingSettings, ILogger<GoogleOptimizeRouteService> logger, IHttpClient httpClient)
        {
            _mappingSettings = mappingSettings?.Value;
            _logger = logger;
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<OptimizeRouteResult> OptimizeRoute(OptimizeRouteCriteria criteria)
        {
            var requestIds = new List<Guid>();

            if (!string.IsNullOrEmpty(_mappingSettings?.GoogleMapsApiKey))
            {
                try
                {
                    return await GetRetryPolicy().ExecuteAsync(async () =>
                    {
                        var googleResponse = await _httpClient.GetAsync(GenerateGoogleApiUrl(criteria));

                        if (googleResponse.IsSuccessStatusCode)
                        {
                            var content = await googleResponse.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<GoogleDirectionsResponse>(content);

                            if (result.Status == GoogleDirectionsResponse.OkStatus)
                            {
                                requestIds.AddRange(result.Routes.SelectMany(r => r.WaypointOrder).Select(waypointIndex => criteria.Waypoints[waypointIndex].RequestId));

                                return new OptimizeRouteResult { RequestIds = requestIds, Distance = result.TotalDistance, Duration = result.TotalDuration };
                            }
                        }
                        else
                        {
                            // for now we fire an exception to force a Polly retry. We could review this later once we have logging to determine specific codes
                            // that represent errors
                            throw new Exception("Non success code");
                        }                  

                        return null;
                    });
                }
                catch
                {
                    // todo - handle other status codes and logging
                }                
            }

            return null;
        }

        private Policy GetRetryPolicy()
        {
            return Policy
                .Handle<Exception>()
                .RetryAsync(2); // in total we will make 3 attempts
        }

        private string GenerateGoogleApiUrl(OptimizeRouteCriteria criteria)
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
