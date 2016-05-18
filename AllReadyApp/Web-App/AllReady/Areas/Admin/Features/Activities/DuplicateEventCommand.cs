using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class DuplicateEventCommand : IAsyncRequest<int>
    {
        public int FromEventId { get; set; }
        public int ToEventId {get; set;}
    }
}
