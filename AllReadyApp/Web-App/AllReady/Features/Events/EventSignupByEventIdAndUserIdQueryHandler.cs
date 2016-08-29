using System.Linq;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Event
{
    public class EventSignupByEventIdAndUserIdQueryHandler : IRequestHandler<EventSignupByEventIdAndUserIdQuery, EventSignup>
    {
        private readonly AllReadyContext dataContext;

        public EventSignupByEventIdAndUserIdQueryHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public EventSignup Handle(EventSignupByEventIdAndUserIdQuery message)
        {
            return this.dataContext.EventSignup
                .Include(z => z.User)
                .Include(x => x.Event)
                .Include(x => x.Event.UsersSignedUp)
                .Where(x => x.Event.Id == message.EventId)
                .SingleOrDefault(x => x.User.Id == message.UserId);
        }
    }
}
