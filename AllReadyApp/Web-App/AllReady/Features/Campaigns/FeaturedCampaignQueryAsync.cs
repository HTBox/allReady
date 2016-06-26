using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class FeaturedCampaignQueryAsync : IAsyncRequest<CampaignSummaryViewModel>
    {
    }
}