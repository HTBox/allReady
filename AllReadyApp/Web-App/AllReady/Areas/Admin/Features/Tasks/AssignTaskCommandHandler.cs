using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class AssignTaskCommandHandler : AsyncRequestHandler<AssignTaskCommand>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        public AssignTaskCommandHandler(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        protected override async Task HandleCore(AssignTaskCommand message)
        {
            var task = await _context.Tasks.SingleAsync(c => c.Id == message.TaskId).ConfigureAwait(false);

            var taskSignups = new List<TaskSignup>();

            //New Items, if not in collection add them, save that list for the pub-event
            foreach (var userId in message.UserIds)
            {
                var taskSignup = task.AssignedVolunteers.SingleOrDefault(a => a.User.Id == userId);
                if (taskSignup != null) continue;

                var user = await _context.Users.SingleAsync(u => u.Id == userId);
                taskSignup = new TaskSignup
                {
                    Task = task,
                    User = user,
                    AdditionalInfo = string.Empty,
                    Status = TaskStatus.Assigned.ToString(),
                    StatusDateTimeUtc = DateTime.UtcNow
                };

                task.AssignedVolunteers.Add(taskSignup);
                taskSignups.Add(taskSignup);
            }

            //Remove task signups where the the user id is not included in the current list of assigned user id's
            var taskSignupsToRemove = task.AssignedVolunteers.Where(taskSignup => message.UserIds.All(uid => uid != taskSignup.User.Id)).ToList();
            taskSignupsToRemove.ForEach(taskSignup => task.AssignedVolunteers.Remove(taskSignup));

            await _context.SaveChangesAsync().ConfigureAwait(false);

            await _mediator.PublishAsync(new TaskAssignedToVolunteersNotification
            {
                TaskId = message.TaskId,
                NewlyAssignedVolunteers = taskSignups.Select(x => x.User.Id).ToList()
            })
            .ConfigureAwait(false);
        }

        
    }
}