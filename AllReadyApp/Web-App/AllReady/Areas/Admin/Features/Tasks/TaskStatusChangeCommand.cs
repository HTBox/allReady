using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class TaskStatusChangeCommand : IRequest
    {
        public int TaskId { get; set; }
        public string UserId { get; set; }
        public TaskStatus TaskStatus { get; set; }
    }

    /// <summary>
    /// Valid status values for a task signup. See
    /// the TaskStatusChangeHandler for the rules
    /// around moving from one status to another.
    /// </summary>
    public enum TaskStatus
    {
        Assigned,
        Accepted,
        Rejected,
        Completed,
        CanNotComplete
    }
}
