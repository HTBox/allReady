using AllReady.Models;
using MediatR;

namespace AllReady.Features.Activity
{
    public class ActivitySignupByActivityIdAndUserIdQueryHandler : IRequestHandler<ActivitySignupByActivityIdAndUserIdQuery, ActivitySignup>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public ActivitySignupByActivityIdAndUserIdQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public ActivitySignup Handle(ActivitySignupByActivityIdAndUserIdQuery message)
        {
            return dataAccess.GetActivitySignup(message.ActivityId, message.UserId);
        }
    }
}
