using MediatR;

namespace AllReady.Features.Notifications
{
    public class EventDetailForNotificationQueryAsync : IAsyncRequest<EventDetailForNotificationModel>
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
    }
}
