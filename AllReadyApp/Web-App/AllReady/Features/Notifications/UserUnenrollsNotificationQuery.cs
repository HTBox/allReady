using MediatR;

namespace AllReady.Features.Notifications
{
    public class UserUnenrollsNotificationQuery : IRequest<UserUnenrollsNotificationModel>
    {
        public int ActivityId { get; set; }
    }
}
