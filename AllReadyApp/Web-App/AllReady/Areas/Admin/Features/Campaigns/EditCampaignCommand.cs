using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class EditCampaignCommand : IAsyncRequest<int>
    {
        public CampaignSummaryModel Campaign {get; set;}
    }
}
