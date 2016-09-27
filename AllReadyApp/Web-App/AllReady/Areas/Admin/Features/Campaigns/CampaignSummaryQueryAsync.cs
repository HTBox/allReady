using AllReady.Areas.Admin.ViewModels.Campaign;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignSummaryQueryAsync : IAsyncRequest<CampaignSummaryViewModel>
    {
        public int CampaignId { get; set; }
    }
}
