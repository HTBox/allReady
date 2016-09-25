using MediatR;

namespace AllReady.Features.Tasks
{
    public class DeleteTaskCommand : IAsyncRequest
    {
        public int TaskId { get; set; }
    }
}
