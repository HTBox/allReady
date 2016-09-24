using AllReady.Areas.Admin.ViewModels.Campaign;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignDetailQueryAsync : IAsyncRequest<CampaignDetailViewModel>
    {
        public int CampaignId { get; set; }
    }
}
