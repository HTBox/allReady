using System.Collections.Generic;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.ClosestLocation
{
    public class ClosestLocationsQuery : IRequest<List<Models.ClosestLocation>>
    {
        public LocationQuery LocationQuery { get; set; }
    }
}
