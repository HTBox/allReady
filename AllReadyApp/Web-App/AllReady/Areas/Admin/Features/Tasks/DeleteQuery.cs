using AllReady.Areas.Admin.ViewModels.Task;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DeleteQuery : IAsyncRequest<DeleteViewModel>
    {
        public int TaskId { get; set; }
    }
}
