using System.Collections.Generic;
using AllReady.ViewModels.Campaign;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class AuthorizedCampaignsQuery : IAsyncRequest<List<ManageCampaignViewModel>>
    {
        public string UserId { get; set; }
    }
}
