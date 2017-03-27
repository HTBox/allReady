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
            var volunteerTask = await _context.VolunteerTasks.SingleAsync(c => c.Id == message.VolunteerTaskId);

            var volunteerTaskSignups = new List<VolunteerTaskSignup>();

            //New Items, if not in collection add them, save that list for the pub-event
            foreach (var userId in message.UserIds)
            {
                var volunteerTaskSignup = volunteerTask.AssignedVolunteers.SingleOrDefault(a => a.User.Id == userId);
                if (volunteerTaskSignup != null) continue;

                var user = await _context.Users.SingleAsync(u => u.Id == userId);
                volunteerTaskSignup = new VolunteerTaskSignup
                {
                    VolunteerTask = volunteerTask,
                    User = user,
                    AdditionalInfo = string.Empty,
                    Status = VolunteerTaskStatus.Assigned,
                    StatusDateTimeUtc = DateTimeUtcNow()
                };

                volunteerTask.AssignedVolunteers.Add(volunteerTaskSignup);
                volunteerTaskSignups.Add(volunteerTaskSignup);
            }

            //Remove task signups where the the user id is not included in the current list of assigned user id's
            var volunteerTaskSignupsToRemove = volunteerTask.AssignedVolunteers.Where(volunteerTaskSignup => message.UserIds.All(uid => uid != volunteerTaskSignup.User.Id)).ToList();
            volunteerTaskSignupsToRemove.ForEach(volunteerTaskSignup => volunteerTask.AssignedVolunteers.Remove(volunteerTaskSignup));

            await _context.SaveChangesAsync();

            await _mediator.PublishAsync(new VolunteerTaskAssignedToVolunteersNotification
            {
                VolunteerTaskId = message.VolunteerTaskId,
                NewlyAssignedVolunteers = volunteerTaskSignups.Select(x => x.User.Id).ToList()
            });
        }        
    }
}