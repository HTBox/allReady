using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using MediatR;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class CreateCampaignManagerInviteCommand : IAsyncRequest<int>
    {
        public CampaignManagerInviteViewModel Invite { get; set; }
        public string UserId { get; set; }
    }
}
