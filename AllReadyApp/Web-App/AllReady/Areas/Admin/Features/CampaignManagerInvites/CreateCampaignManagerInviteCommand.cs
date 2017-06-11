using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using MediatR;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class CreateCampaignManagerInviteCommand : IAsyncRequest
    {
        public CampaignManagerInviteViewModel Invite { get; set; }
        public string UserId { get; set; }
    }
}
