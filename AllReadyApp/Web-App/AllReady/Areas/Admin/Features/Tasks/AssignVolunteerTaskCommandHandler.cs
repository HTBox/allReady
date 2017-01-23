using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class AssignVolunteerTaskCommandHandler : AsyncRequestHandler<AssignVolunteerTaskCommand>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;
        public Func<DateTime> 
            DateTimeUtcNow = () => DateTime.UtcNow;

        public AssignVolunteerTaskCommandHandler(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        protected override async Task HandleCore(AssignVolunteerTaskCommand message)
        {
            var @task = await _context.Tasks.SingleAsync(c => c.Id == message.VolunteerTaskId);

            var taskSignups = new List<VolunteerTaskSignup>();

            //New Items, if not in collection add them, save that list for the pub-event
            foreach (var userId in message.UserIds)
            {
                var taskSignup = @task.AssignedVolunteers.SingleOrDefault(a => a.User.Id == userId);
                if (taskSignup != null) continue;

                var user = await _context.Users.SingleAsync(u => u.Id == userId);
                taskSignup = new VolunteerTaskSignup
                {
                    VolunteerTask = @task,
                    User = user,
                    AdditionalInfo = string.Empty,
                    Status = VolunteerTaskStatus.Assigned,
                    StatusDateTimeUtc = DateTimeUtcNow()
                };

                @task.AssignedVolunteers.Add(taskSignup);
                taskSignups.Add(taskSignup);
            }

            //Remove task signups where the the user id is not included in the current list of assigned user id's
            var taskSignupsToRemove = @task.AssignedVolunteers.Where(taskSignup => message.UserIds.All(uid => uid != taskSignup.User.Id)).ToList();
            taskSignupsToRemove.ForEach(taskSignup => @task.AssignedVolunteers.Remove(taskSignup));

            await _context.SaveChangesAsync();

            await _mediator.PublishAsync(new TaskAssignedToVolunteersNotification
            {
                TaskId = message.VolunteerTaskId,
                NewlyAssignedVolunteers = taskSignups.Select(x => x.User.Id).ToList()
            });
        }        
    }
}