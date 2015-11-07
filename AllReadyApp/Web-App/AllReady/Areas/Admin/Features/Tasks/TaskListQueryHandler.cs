using AllReady.Areas.Admin.ViewModels;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class TaskListQueryHandler : IRequestHandler<TaskListQuery, IEnumerable<TaskSummaryViewModel>>
    {
        private AllReadyContext _context;

        public TaskListQueryHandler(AllReadyContext context)
        {
            _context = context;

        }
        public IEnumerable<TaskSummaryViewModel> Handle(TaskListQuery message)
        {
            var tasks = _context.Tasks
                .Where(t => t.Activity.Id == message.ActivityId)
                .Select(t => new TaskSummaryViewModel()
                {
                    Id = t.Id,
                    Name = t.Name,
                    StartDateTime = t.StartDateTimeUtc,
                    EndDateTime = t.EndDateTimeUtc,
                    IsUserSignedUpForTask = false
                });
            //TODO: Add AssignedVolunteers here.
            return tasks;
        }
    }
}
