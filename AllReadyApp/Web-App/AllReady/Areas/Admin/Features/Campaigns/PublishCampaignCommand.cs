using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class PublishCampaignCommand : IAsyncRequest
    {
        public int CampaignId {get; set;}
    }
}
