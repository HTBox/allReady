using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using MediatR;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class EventManagerInviteQuery : IAsyncRequest<EventManagerInviteViewModel>
    {
        public int EventId { get; set; }
    }
}
