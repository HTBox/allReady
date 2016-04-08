using MediatR;

namespace AllReady.Features.Notifications
{
    public class ActivityDetailForNotificationQueryAsync : IAsyncRequest<ActivityDetailForNotificationModel>
    {
        public int ActivityId { get; set; }
        public string UserId { get; set; }
    }
}
