using System.Collections.Generic;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using System.Linq;

namespace AllReady.Features.Activity
{
    public class ActivitiesWithUnlockedCampaignsQueryHandler : IRequestHandler<ActivitiesWithUnlockedCampaignsQuery, List<ActivityViewModel>>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public ActivitiesWithUnlockedCampaignsQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public List<ActivityViewModel> Handle(ActivitiesWithUnlockedCampaignsQuery message)
        {
            return dataAccess.Activities.Where(c => !c.Campaign.Locked)
                .Select(a => new ActivityViewModel(a))
                .ToList();
        }
    }
}
