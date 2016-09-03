using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Events
{
    public class EventsByGeographyQueryHandler : IRequestHandler<EventsByGeographyQuery, List<Models.Event>>
    {
        private readonly AllReadyContext dataContext;

        public EventsByGeographyQueryHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public List<Models.Event> Handle(EventsByGeographyQuery message)
        {
            return this.dataContext.Events.FromSql("EXEC GetClosestEvents {0}, {1}, {2}, {3}", message.Latitude, message.Longitude, 50, message.Miles).ToList();
        }
    }
}
