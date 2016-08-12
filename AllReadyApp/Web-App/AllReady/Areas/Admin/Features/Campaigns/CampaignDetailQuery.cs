using AllReady.Areas.Admin.ViewModels.Campaign;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignDetailQuery : IAsyncRequest<CampaignDetailViewModel>
    {
        public int CampaignId { get; set; }
    }
}
