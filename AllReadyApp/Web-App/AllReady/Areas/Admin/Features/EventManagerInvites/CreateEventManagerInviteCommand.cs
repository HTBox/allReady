using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using MediatR;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class CreateEventManagerInviteCommand : IAsyncRequest<int>
    {
        public EventManagerInviteViewModel Invite { get; set; }
        public string UserId { get; set; }
        public string EventName { get; set; }
        public string SenderName { get; set; }
        public string AcceptUrl { get; set; }
    }
}
