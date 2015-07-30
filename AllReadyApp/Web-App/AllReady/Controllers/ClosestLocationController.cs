using Microsoft.AspNet.Mvc;
using System.Collections.Generic;

using AllReady.Services;
using AllReady.Models;

namespace AllReady.Controllers
{
    [Route("api/closest")]
    public class ClosestLocationsController : Controller
    {
        private IClosestLocations _closestLocations;

        public ClosestLocationsController(IClosestLocations closestLocations)
        {
            _closestLocations = closestLocations;
        }

        [HttpGet("{lat}/{lon}/{distance}/{count}")]
        public IEnumerable<ClosestLocation> Get(double lat, double lon, int distance, int count)
        {
            var results = _closestLocations.GetClosestLocations(new LocationQuery
            {
                Distance = distance,
                Latitude = lat,
                Longitude = lon,
                MaxRecordsToReturn = count
            });

            return results;
        }
    }
}
