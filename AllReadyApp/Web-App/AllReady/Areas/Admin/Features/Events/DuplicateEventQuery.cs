using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class DuplicateEventQuery : IAsyncRequest<DuplicateEventModel>
    {
        public int EventId { get; set; }
    }
}
