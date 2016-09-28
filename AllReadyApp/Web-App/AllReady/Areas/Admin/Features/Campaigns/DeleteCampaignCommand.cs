using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class DeleteCampaignCommand : IAsyncRequest
    {
        public int CampaignId {get; set;}
    }
}
