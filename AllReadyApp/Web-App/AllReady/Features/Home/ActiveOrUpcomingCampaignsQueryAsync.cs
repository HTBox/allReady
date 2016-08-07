using System.Collections.Generic;
using AllReady.ViewModels.Home;
using MediatR;

namespace AllReady.Features.Home
{
    public class ActiveOrUpcomingCampaignsQueryAsync : IAsyncRequest<List<ActiveOrUpcomingCampaign>>
    {
    }
}
