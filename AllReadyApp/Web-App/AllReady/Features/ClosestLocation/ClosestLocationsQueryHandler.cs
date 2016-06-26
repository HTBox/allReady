using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Features.ClosestLocation
{
    public class ClosestLocationsQueryHandler : IRequestHandler<ClosestLocationsQuery, List<Models.ClosestLocation>>
    {
        private readonly AllReadyContext _context;

        public ClosestLocationsQueryHandler(AllReadyContext context)
        {
            this._context = context;
        }

        public List<Models.ClosestLocation> Handle(ClosestLocationsQuery message)
        {
            return _context.ClosestLocations.FromSql(
                "EXEC GetClosestLocations {0}, {1}, {2}, {3}",
                message.LocationQuery.Latitude,
                message.LocationQuery.Longitude,
                message.LocationQuery.MaxRecordsToReturn ?? 10,
                message.LocationQuery.Distance ?? 10
            ).ToList();
        }
    }
}
