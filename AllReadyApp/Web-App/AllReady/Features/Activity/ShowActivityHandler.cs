using System.Security.Claims;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Activity
{
    public class ShowActivityHandler : IRequestHandler<ShowActivityCommand, ActivityViewModel>
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public ShowActivityHandler(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public ActivityViewModel Handle(ShowActivityCommand message)
        {
            var activity = _dataAccess.GetActivity(message.ActivityId);

            if (activity == null || activity.Campaign.Locked)
            {
                return null;
            }

            return new ActivityViewModel(activity)
                .WithUserInfo(activity, ClaimsPrincipal.Current, _dataAccess);
        }
    }
}
