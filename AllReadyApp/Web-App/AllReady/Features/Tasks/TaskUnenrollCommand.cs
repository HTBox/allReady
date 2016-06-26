using MediatR;

namespace AllReady.Features.Tasks
{
    public class TaskUnenrollCommand : IAsyncRequest<TaskUnenrollResult>
    {
        public int TaskId { get; set; }
        public string UserId { get; set; }
    }
}
