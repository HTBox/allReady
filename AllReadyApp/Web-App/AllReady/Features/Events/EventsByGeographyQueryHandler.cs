using System.Collections.Generic;
using System.Linq;
using AllReady.ExtensionWrappers;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Events
{
    public class EventsByGeographyQueryHandler : IRequestHandler<EventsByGeographyQuery, List<Models.Event>>
    {
        private readonly AllReadyContext dataContext;
        private readonly IFromSqlWrapper extensionWrapper;

        public EventsByGeographyQueryHandler(AllReadyContext dataContext, IFromSqlWrapper extensionWrapper)
        {
            this.dataContext = dataContext;
            this.extensionWrapper = extensionWrapper;
        }

        public List<Models.Event> Handle(EventsByGeographyQuery message)
        {
            var events = extensionWrapper.FromSql(dataContext.Events, "EXEC GetClosestEvents {0}, {1}, {2}, {3}",
                message.Latitude, message.Longitude, 50, message.Miles);
            return events.ToList();
        }
    }
}
