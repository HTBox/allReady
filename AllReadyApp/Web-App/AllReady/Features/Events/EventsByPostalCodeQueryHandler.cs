using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Events
{
    public class EventsByPostalCodeQueryHandler : IRequestHandler<EventsByPostalCodeQuery, List<Models.Event>>
    {
        private readonly AllReadyContext dataContext;

        public EventsByPostalCodeQueryHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public List<Models.Event> Handle(EventsByPostalCodeQuery message)
        {
            return dataContext.Events.FromSql("EXEC GetClosestEventsByPostalCode '{0}', {1}, {2}", message.PostalCode, 50, message.Distance).Include(a => a.Campaign).ToList();
        }
    }
}
