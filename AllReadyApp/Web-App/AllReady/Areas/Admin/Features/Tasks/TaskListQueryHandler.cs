using System.Collections.Generic;
using System.Linq;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class TaskListQueryHandler : IRequestHandler<TaskListQuery, IEnumerable<TaskSummaryModel>>
    {
        private AllReadyContext _context;

        public TaskListQueryHandler(AllReadyContext context)
        {
            _context = context;

        }
        public IEnumerable<TaskSummaryModel> Handle(TaskListQuery message)
        {
            var tasks = _context.Tasks
                .Where(t => t.Event.Id == message.EventId)
                .Select(t => new TaskSummaryModel()
                {
                    Id = t.Id,
                    Name = t.Name,
                    StartDateTime = t.StartDateTime,
                    EndDateTime = t.EndDateTime,
                    NumberOfVolunteersRequired = t.NumberOfVolunteersRequired,
                    IsUserSignedUpForTask = false
                });
            //TODO: Add AssignedVolunteers here.
            return tasks;
        }
    }
}
