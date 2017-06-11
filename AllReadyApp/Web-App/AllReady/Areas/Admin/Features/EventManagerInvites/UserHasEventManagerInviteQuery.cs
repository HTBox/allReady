using MediatR;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class UserHasEventManagerInviteQuery : IAsyncRequest<bool>
    {
        public string InviteeEmail { get; set; }
        public int EventId { get; set; }
    }
}
