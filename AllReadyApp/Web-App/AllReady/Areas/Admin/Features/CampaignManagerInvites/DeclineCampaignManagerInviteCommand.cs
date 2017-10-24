using MediatR;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class DeclineCampaignManagerInviteCommand : IAsyncRequest
    {
        public int CampaignManagerInviteId { get; set; }
    }
}
