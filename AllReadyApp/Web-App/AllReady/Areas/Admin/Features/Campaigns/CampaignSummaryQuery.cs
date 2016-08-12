using AllReady.Areas.Admin.ViewModels.Campaign;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignSummaryQuery : IAsyncRequest<CampaignSummaryModel>
    {
        public int CampaignId { get; set; }
    }
}
