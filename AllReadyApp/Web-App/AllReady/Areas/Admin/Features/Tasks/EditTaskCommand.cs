using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditTaskCommand : IRequest<int>
    {
        public TaskEditModel Task {get; set;}
    }
}
