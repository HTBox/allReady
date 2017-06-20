using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using MediatR;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class CampaignManagerInviteQuery : IAsyncRequest<CampaignManagerInviteViewModel>
    {
        public int CampaignId { get; set; }
    }
}
