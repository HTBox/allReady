using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DeleteTaskCommandAsync : IAsyncRequest
    {
        public int TaskId {get; set;}
    }
}
