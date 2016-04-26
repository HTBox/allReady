using AllReady.Models;
using MediatR;

namespace AllReady.Features.Event
{
    public class EventByIdQueryHandler : IRequestHandler<EventByIdQuery, Models.Event>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public EventByIdQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public Models.Event Handle(EventByIdQuery message)
        {
            return dataAccess.GetEvent(message.EventId);
        }
    }
}
