using System.Collections.Generic;
using AllReady.Models;
using Microsoft.AspNet.Mvc;

namespace AllReady.Controllers
{
    [Route("api/closest")]
    public class ClosestLocationsController : Controller
    {
        private IAllReadyDataAccess _dataAccess;

        public ClosestLocationsController(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [HttpGet("{lat}/{lon}/{distance}/{count}")]
        public IEnumerable<ClosestLocation> Get(double lat, double lon, int distance, int count)
        {
            var results = _dataAccess.GetClosestLocations(new LocationQuery
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
