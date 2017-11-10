using MediatR;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class AcceptEventManagerInviteCommand : IAsyncRequest
    {
        public int EventManagerInviteId { get; set; }
    }
}
