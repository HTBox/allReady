using MediatR;

namespace AllReady.Features.Campaigns
{
    public class CampaignByCampaignIdQuery : IRequest<Models.Campaign>
    {
        public int CampaignId { get; set; }
    }
}
