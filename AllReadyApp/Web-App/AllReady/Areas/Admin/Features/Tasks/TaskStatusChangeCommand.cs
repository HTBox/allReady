using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class TaskStatusChangeCommand : IRequest
    {
        public int TaskId { get; set; }
        public string UserId { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public string TaskStatusDescription { get; set; }
    }

    public enum TaskStatus
    {
        Assigned,
        Accepted,
        Rejected,
        Completed,
        CanNotComplete
    }
}
