using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.ClosestLocation
{
    public class ClosestLocationsQueryHandler : IRequestHandler<ClosestLocationsQuery, List<Models.ClosestLocation>>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public ClosestLocationsQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public List<Models.ClosestLocation> Handle(ClosestLocationsQuery message)
        {
            return dataAccess.GetClosestLocations(message.LocationQuery).ToList();
        }
    }
}
