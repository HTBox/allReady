using MediatR;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class CreateEventManagerCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public int EventId { get; set; }
    }
}
