using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using MediatR;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class CampaignManagerInviteDetailQuery : IAsyncRequest<CampaignManagerInviteDetailsViewModel>
    {
        public int CampaignManagerInviteId { get; set; }
    }
}
