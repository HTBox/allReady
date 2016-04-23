using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Event
{
    public class EventsByGeographyQueryHandler : IRequestHandler<EventsByGeographyQuery, List<Models.Event>>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public EventsByGeographyQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public List<Models.Event> Handle(EventsByGeographyQuery message)
        {
            return dataAccess.EventsByGeography(message.Latitude, message.Longitude, message.Miles).ToList();
        }
    }
}
