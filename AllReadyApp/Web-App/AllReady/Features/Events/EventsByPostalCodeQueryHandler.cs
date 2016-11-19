using System.Collections.Generic;
using System.Linq;
using AllReady.ExtensionWrappers;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Events
{
    public class EventsByPostalCodeQueryHandler : IRequestHandler<EventsByPostalCodeQuery, List<Models.Event>>
    {
        private readonly AllReadyContext dataContext;
        private readonly IFromSqlWrapper extensionWrapper;

        public EventsByPostalCodeQueryHandler(AllReadyContext dataContext, IFromSqlWrapper extensionWrapper)
        {
            this.dataContext = dataContext;
            this.extensionWrapper = extensionWrapper;
        }

        public List<Models.Event> Handle(EventsByPostalCodeQuery message)
        {
            var events = extensionWrapper.FromSql(dataContext.Events,
                "EXEC GetClosestEventsByPostalCode '{0}', {1}, {2}", message.PostalCode, 50, message.Distance);
            return events.Include(a => a.Campaign).ToList();
        }
    }
}
