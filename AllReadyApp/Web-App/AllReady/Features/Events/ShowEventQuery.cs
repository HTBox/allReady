using AllReady.ViewModels.Event;
using MediatR;

namespace AllReady.Features.Events
{
    public class ShowEventQuery : IAsyncRequest<EventViewModel>
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
    }
}