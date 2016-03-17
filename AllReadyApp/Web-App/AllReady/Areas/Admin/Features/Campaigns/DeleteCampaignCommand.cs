using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class DeleteCampaignCommand : IRequest
    {
        public int CampaignId {get; set;}
    }
}
