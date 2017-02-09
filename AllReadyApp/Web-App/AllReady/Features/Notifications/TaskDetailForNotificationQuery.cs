using MediatR;

namespace AllReady.Features.Notifications
{
    public class TaskDetailForNotificationQuery : IAsyncRequest<TaskDetailForNotificationModel>
    {
        public int VolunteerTaskId { get; set; }
        public string UserId { get; set; }
    }
}
