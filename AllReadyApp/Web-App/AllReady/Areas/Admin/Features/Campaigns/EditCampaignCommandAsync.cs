using AllReady.Areas.Admin.ViewModels.Campaign;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class EditCampaignCommandAsync : IAsyncRequest<int>
    {
        public CampaignSummaryViewModel Campaign {get; set;}
    }
}
