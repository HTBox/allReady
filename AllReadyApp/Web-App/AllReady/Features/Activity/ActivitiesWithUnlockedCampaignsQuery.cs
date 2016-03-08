using System.Collections.Generic;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Activity
{
    public class ActivitiesWithUnlockedCampaignsQuery : IRequest<List<ActivityViewModel>>
    {
    }
}
