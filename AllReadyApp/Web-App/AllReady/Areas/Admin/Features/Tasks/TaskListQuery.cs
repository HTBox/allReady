using System.Collections.Generic;
using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class TaskListQuery : IRequest<IEnumerable<TaskSummaryModel>>
    {
        public int ActivityId { get; set; }
    }
}
