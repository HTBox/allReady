using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class LockUnlockCampaignCommand : IAsyncRequest
    {
        public int CampaignId {get; set;}
    }
}
