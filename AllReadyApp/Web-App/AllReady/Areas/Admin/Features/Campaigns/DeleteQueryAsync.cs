using AllReady.Areas.Admin.ViewModels.Campaign;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class DeleteQueryAsync : IAsyncRequest<DeleteViewModel>
    {
        public int CampaignId { get; set; }
    }
}
