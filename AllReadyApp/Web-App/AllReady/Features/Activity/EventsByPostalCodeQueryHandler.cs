using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Event
{
    public class EventsByPostalCodeQueryHandler : IRequestHandler<EventsByPostalCodeQuery, List<Models.Event>>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public EventsByPostalCodeQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public List<Models.Event> Handle(EventsByPostalCodeQuery message)
        {
            return dataAccess.EventsByPostalCode(message.PostalCode, message.Distance).ToList();
        }
    }
}
