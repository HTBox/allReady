using MediatR;

namespace AllReady.Features.Events
{
    public class EventByIdQuery : IRequest<Models.Event>
    {
        public int EventId { get; set; }
    }
}
