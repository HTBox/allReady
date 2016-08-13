using AllReady.Areas.Admin.ViewModels.Event;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EventDetailQuery : IAsyncRequest<EventDetailViewModel>
    {
        public int EventId { get; set; }
    }
}
