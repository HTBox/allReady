using MediatR;

namespace AllReady.Features.Tasks
{
    public class TaskUnenrollCommand : IAsyncRequest<TaskSignupResult>
    {
        public int TaskId { get; set; }
        public string UserId { get; set; }
    }
}
