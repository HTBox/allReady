using AllReady.Areas.Admin.ViewModels.Campaign;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class EditCampaignCommand : IAsyncRequest<int>
    {
        public CampaignSummaryModel Campaign {get; set;}
    }
}
