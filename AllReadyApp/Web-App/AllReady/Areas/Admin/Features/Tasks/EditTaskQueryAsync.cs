using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditTaskQueryAsync : IAsyncRequest<TaskEditModel>
    {
        public int TaskId { get; set; }
    }
}
