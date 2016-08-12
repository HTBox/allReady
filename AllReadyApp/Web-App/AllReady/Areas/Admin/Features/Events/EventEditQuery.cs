using AllReady.Areas.Admin.ViewModels.Event;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EventEditQuery : IAsyncRequest<EventEditModel>
    {
        public int EventId { get; set; }
    }
}
