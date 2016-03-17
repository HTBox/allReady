using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditTaskQuery : IRequest<TaskEditModel>
    {
        public int TaskId { get; set; }
    }
}
