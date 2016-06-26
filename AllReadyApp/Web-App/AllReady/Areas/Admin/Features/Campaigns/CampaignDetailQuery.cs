using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignDetailQuery : IAsyncRequest<CampaignDetailModel>
    {
        public int CampaignId { get; set; }
    }
}
