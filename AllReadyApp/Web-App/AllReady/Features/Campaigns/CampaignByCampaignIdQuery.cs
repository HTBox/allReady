using MediatR;

namespace AllReady.Features.Campaigns
{
    public class CampaignByCampaignIdQuery : IAsyncRequest<Models.Campaign>
    {
        public int CampaignId { get; set; }
    }
}
