using System.Linq;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Activity
{
    public class GetMyActivitiesQueryHandler : IRequestHandler<GetMyActivitiesQuery, MyActivitiesResultsScreenViewModel>
    {
        private readonly IAllReadyDataAccess _allReadyDataAccess;

        public GetMyActivitiesQueryHandler(IAllReadyDataAccess allReadyDataAccess)
        {
            _allReadyDataAccess = allReadyDataAccess;
        }


        public MyActivitiesResultsScreenViewModel Handle(GetMyActivitiesQuery message)
        {
            var myActivities = _allReadyDataAccess.GetActivitySignups(message.UserId).Where(a => !a.Activity.Campaign.Locked);
            var signedUp = myActivities.Select(a => new ActivityViewModel(a.Activity)).ToList();
            return new MyActivitiesResultsScreenViewModel("My Activities", signedUp);
        }
    }
}
