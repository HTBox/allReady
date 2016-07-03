using AllReady.ViewModels.Home;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class FeaturedCampaignQueryAsync : IAsyncRequest<CampaignSummaryViewModel>
    {
    }
}