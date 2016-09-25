using AllReady.ViewModels.Home;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class FeaturedCampaignQuery : IAsyncRequest<CampaignSummaryViewModel>
    {
    }
}