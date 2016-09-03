using System.Collections.Generic;
using MediatR;

namespace AllReady.Features.Events
{
    public class EventsByGeographyQuery : IRequest<List<Models.Event>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Miles { get; set; }
    }
}
