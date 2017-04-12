using AllReady.Areas.Admin.ViewModels.Campaign;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignUsersDetailsQuery : IAsyncRequest<CampaignUsersDetailsViewModel>
    {
        public int CampaignId { get; set; }
    }
}
