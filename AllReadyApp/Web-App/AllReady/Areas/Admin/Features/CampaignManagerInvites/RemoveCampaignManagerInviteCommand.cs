using MediatR;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class RemoveCampaignManagerInviteCommand : IAsyncRequest
    {
        public int InviteId { get; set; }
    }
}
