using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using MediatR;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class CreateEventManagerInviteCommand : IAsyncRequest
    {
        public EventManagerInviteViewModel Invite { get; set; }
        public string UserId { get; set; }
        public string SenderName { get; set; }
        public string RegisterUrl { get; set; }
        public bool IsInviteeRegistered { get; set; }
    }
}
