using AllReady.Areas.Admin.ViewModels.Event;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EventEditQuery : IAsyncRequest<EventEditViewModel>
    {
        public int EventId { get; set; }
    }
}
