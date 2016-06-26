using MediatR;

namespace AllReady.Features.Tasks
{
    public class DeleteTaskCommandAsync : IAsyncRequest
    {
        public int TaskId { get; set; }
    }
}
