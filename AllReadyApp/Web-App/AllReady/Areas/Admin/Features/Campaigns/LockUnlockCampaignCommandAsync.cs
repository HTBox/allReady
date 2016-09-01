using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class LockUnlockCampaignCommandAsync : IAsyncRequest
    {
        public int CampaignId {get; set;}
    }
}
