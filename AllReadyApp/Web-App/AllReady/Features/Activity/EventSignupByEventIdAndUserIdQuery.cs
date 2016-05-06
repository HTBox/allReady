using AllReady.Models;
using MediatR;

namespace AllReady.Features.Event
{
    public class EventSignupByEventIdAndUserIdQuery : IRequest<EventSignup>
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
    }
}
