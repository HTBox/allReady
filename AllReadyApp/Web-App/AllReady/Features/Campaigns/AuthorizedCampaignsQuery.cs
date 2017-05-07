using System.Collections.Generic;
using AllReady.ViewModels.Campaign;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class AuthorizedCampaignsQuery : IAsyncRequest<List<CampaignViewModel>>
    {
        public string UserId { get; set; }
    }
}
