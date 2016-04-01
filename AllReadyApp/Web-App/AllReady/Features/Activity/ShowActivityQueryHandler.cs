using AllReady.Models;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Activity
{
    public class ShowActivityQueryHandler : IRequestHandler<ShowActivityQuery, ActivityViewModel>
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public ShowActivityQueryHandler(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public ActivityViewModel Handle(ShowActivityQuery message)
        {
            var activity = _dataAccess.GetActivity(message.ActivityId);

            if (activity == null || activity.Campaign.Locked)
            {
                return null;
            }

            return new ActivityViewModel(activity)
                .WithUserInfo(activity, message.User, _dataAccess);
        }
    }
}
