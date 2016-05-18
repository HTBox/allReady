using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class DuplicateEventQuery : IAsyncRequest<EventDuplicateModel>
    {
        public int EventId { get; set; }
    }
}
