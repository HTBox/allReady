using MediatR;

namespace AllReady.Features.Notifications
{
    public class VolunteerTaskDetailForNotificationQuery : IAsyncRequest<VolunteerTaskDetailForNotificationModel>
    {
        public int VolunteerTaskId { get; set; }
        public string UserId { get; set; }
    }
}
