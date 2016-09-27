using MediatR;

namespace AllReady.Features.Events
{
    public class EventByEventIdQueryAsync : IAsyncRequest<Models.Event>
    {
        public int EventId { get; set; }
    }
}
