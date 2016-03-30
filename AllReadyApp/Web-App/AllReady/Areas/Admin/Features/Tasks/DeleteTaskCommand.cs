using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DeleteTaskCommand : IAsyncRequest
    {
        public int TaskId {get; set;}
    }
}
