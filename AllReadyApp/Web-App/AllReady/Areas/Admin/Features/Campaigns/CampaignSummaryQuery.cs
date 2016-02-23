using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignSummaryQuery : IRequest<CampaignSummaryModel>
    {
        public int CampaignId { get; set; }
    }
}
