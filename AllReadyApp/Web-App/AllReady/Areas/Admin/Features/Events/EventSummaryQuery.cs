using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EventSummaryQuery : IAsyncRequest<EventSummaryModel>
    {
        public int EventId { get; set; }
    }
}
