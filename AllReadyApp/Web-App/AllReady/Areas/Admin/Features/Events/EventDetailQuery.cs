using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EventDetailQuery : IAsyncRequest<EventDetailModel>
    {
        public int EventId { get; set; }
    }
}
