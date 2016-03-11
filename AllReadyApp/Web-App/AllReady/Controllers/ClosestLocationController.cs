using System.Collections.Generic;
using AllReady.Features.ClosestLocation;
using AllReady.Models;
using MediatR;
using Microsoft.AspNet.Mvc;

namespace AllReady.Controllers
{
    [Route("api/closest")]
    public class ClosestLocationsController : Controller
    {
        private readonly IMediator mediator;

        public ClosestLocationsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("{lat}/{lon}/{distance}/{count}")]
        public IEnumerable<ClosestLocation> Get(double latitude, double longtitude, int distance, int count)
        {
            var results = mediator.Send(new ClosestLocationsQuery
            {
                LocationQuery = new LocationQuery { Distance = distance, Latitude = latitude, Longitude = longtitude, MaxRecordsToReturn = count }
            });

            return results;
        }
    }
}
