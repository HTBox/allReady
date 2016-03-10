using System.Collections.Generic;
using MediatR;

namespace AllReady.Features.Activity
{
    public class ActivitiesByGeographyQuery : IRequest<List<Models.Activity>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Miles { get; set; }
    }
}
