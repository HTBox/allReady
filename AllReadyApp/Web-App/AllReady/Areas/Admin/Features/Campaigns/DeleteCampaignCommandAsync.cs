using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class DeleteCampaignCommandAsync : IAsyncRequest
    {
        public int CampaignId {get; set;}
    }
}
