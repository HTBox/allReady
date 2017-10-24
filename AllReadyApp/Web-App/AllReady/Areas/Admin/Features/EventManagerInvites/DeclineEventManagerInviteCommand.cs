using MediatR;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class DeclineEventManagerInviteCommand : IAsyncRequest
    {
        public int EventManagerInviteId { get; set; }
    }
}
