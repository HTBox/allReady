using System.Collections.Generic;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using System.Linq;

namespace AllReady.Features.Activity
{
    public class GetActivitiesWithUnlockedCampaignsQueryHandler : IRequestHandler<GetActivitiesWithUnlockedCampaignsQuery, List<ActivityViewModel>>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public GetActivitiesWithUnlockedCampaignsQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public List<ActivityViewModel> Handle(GetActivitiesWithUnlockedCampaignsQuery message)
        {
            return dataAccess.Activities.Where(c => !c.Campaign.Locked)
                .Select(a => new ActivityViewModel(a))
                .ToList();
        }
    }
}
