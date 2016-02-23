using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DeleteTaskCommand : IRequest
    {
        public int TaskId {get; set;}
    }
}
