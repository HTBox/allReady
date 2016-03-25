using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class TaskQuery : IAsyncRequest<TaskSummaryModel>
    {
        public int TaskId { get; set; }
    }
}
