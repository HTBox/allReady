using MediatR;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class CreateCampaignManagerCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public int CampaignId { get; set; }
    }
}
