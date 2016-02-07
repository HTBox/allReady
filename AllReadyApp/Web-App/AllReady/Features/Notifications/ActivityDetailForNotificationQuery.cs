using MediatR;

namespace AllReady.Features.Notifications
{
    public class ActivityDetailForNotificationQuery : IRequest<ActivityDetailForNotificationModel>
    {
        public int ActivityId { get; set; }
        public string UserId { get; set; }
    }
}
