using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using MediatR;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class CreateEventManagerInviteCommand : IAsyncRequest
    {
        public EventManagerInviteViewModel Invite { get; set; }
        public string UserId { get; set; }
    }
}
