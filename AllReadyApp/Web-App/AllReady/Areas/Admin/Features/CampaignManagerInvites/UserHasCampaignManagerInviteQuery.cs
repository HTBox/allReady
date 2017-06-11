using MediatR;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class UserHasCampaignManagerInviteQuery : IAsyncRequest<bool>
    {
        public string InviteeEmail { get; set; }
        public int CampaignId { get; set; }
    }
}
