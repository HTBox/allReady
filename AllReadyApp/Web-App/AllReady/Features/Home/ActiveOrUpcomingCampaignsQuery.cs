using System.Collections.Generic;
using AllReady.ViewModels.Home;
using MediatR;

namespace AllReady.Features.Home
{
    public class ActiveOrUpcomingCampaignsQuery : IAsyncRequest<List<ActiveOrUpcomingCampaign>>
    {
    }
}
