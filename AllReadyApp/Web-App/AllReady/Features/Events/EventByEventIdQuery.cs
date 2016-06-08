using MediatR;

namespace AllReady.Features.Event
{
    public class EventByIdQuery : IRequest<Models.Event>
    {
        public int EventId { get; set; }
    }
}
