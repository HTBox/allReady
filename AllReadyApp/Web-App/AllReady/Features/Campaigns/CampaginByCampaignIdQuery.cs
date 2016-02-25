using MediatR;

namespace AllReady.Features.Campaigns
{
    public class CampaginByCampaignIdQuery : IRequest<Models.Campaign>
    {
        public int CampaignId { get; set; }
    }
}
