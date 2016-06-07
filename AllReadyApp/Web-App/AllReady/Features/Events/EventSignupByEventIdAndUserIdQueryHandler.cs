using AllReady.Models;
using MediatR;

namespace AllReady.Features.Event
{
    public class EventSignupByEventIdAndUserIdQueryHandler : IRequestHandler<EventSignupByEventIdAndUserIdQuery, EventSignup>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public EventSignupByEventIdAndUserIdQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public EventSignup Handle(EventSignupByEventIdAndUserIdQuery message)
        {
            return dataAccess.GetEventSignup(message.EventId, message.UserId);
        }
    }
}
