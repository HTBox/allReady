using MediatR;

namespace AllReady.Features.Events
{
    public class EventByEventIdQuery : IAsyncRequest<Models.Event>
    {
        public int EventId { get; set; }
    }
}
