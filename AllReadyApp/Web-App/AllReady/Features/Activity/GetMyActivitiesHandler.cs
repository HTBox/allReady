using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Activity
{
    public class GetMyActivitiesHandler: IRequestHandler<GetMyActivitiesCommand, MyActivitiesResultsScreenViewModel>
    {
        private readonly IAllReadyDataAccess _allReadyDataAccess;

        public GetMyActivitiesHandler(IAllReadyDataAccess allReadyDataAccess)
        {
            _allReadyDataAccess = allReadyDataAccess;
        }


        public MyActivitiesResultsScreenViewModel Handle(GetMyActivitiesCommand message)
        {
            var myActivities = _allReadyDataAccess.GetActivitySignups(message.UserId).Where(a => !a.Activity.Campaign.Locked);
            var signedUp = myActivities.Select(a => new ActivityViewModel(a.Activity)).ToList();
            return new MyActivitiesResultsScreenViewModel("My Activities", signedUp);
        }
    }
}
