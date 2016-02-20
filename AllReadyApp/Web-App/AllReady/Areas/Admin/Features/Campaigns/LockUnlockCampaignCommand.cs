using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class LockUnlockCampaignCommand : IRequest
    {
        public int CampaignId {get; set;}
    }
}
