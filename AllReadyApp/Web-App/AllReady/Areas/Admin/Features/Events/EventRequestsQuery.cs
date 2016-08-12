using AllReady.Areas.Admin.Models.EventViewModels;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EventRequestsQuery : IAsyncRequest<EventRequestsViewModel>
    {
        public int EventId { get; set; }
    }
}
