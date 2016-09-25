using AllReady.Areas.Admin.ViewModels.Task;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditTaskQuery : IAsyncRequest<EditViewModel>
    {
        public int TaskId { get; set; }
    }
}
