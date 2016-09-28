using MediatR;

namespace AllReady.Features.Notifications
{
    public class TaskDetailForNotificationQuery : IAsyncRequest<TaskDetailForNotificationModel>
    {
        public int TaskId { get; set; }
        public string UserId { get; set; }
    }
}
