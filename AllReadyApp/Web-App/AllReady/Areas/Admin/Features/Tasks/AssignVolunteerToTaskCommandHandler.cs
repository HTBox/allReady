using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Services;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class AssignVolunteerToTaskCommandHandler : AsyncRequestHandler<AssignVolunteerToTaskCommand>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;
        private IEmailSender _messageSender;
        public Func<DateTime>
            DateTimeUtcNow = () => DateTime.UtcNow;

        public AssignVolunteerToTaskCommandHandler(AllReadyContext context, IMediator mediator, IEmailSender messageSender)
        {
            _context = context;
            _mediator = mediator;
            _messageSender = messageSender;
        }

        protected override async Task HandleCore(AssignVolunteerToTaskCommand message)
        {
            var volunteerTask = await _context.VolunteerTasks.SingleAsync(c => c.Id == message.VolunteerTaskId);

            var volunteerTaskSignups = new List<VolunteerTaskSignup>();

            var volunteerTaskSignup = volunteerTask.AssignedVolunteers.SingleOrDefault(a => a.User.Id == message.UserId);
            if (volunteerTaskSignup != null)
                return;

            var user = await _context.Users.SingleAsync(u => u.Id == message.UserId);
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

            if (message.NotifyUser)
            {
                await _messageSender.SendEmailAsync(user.Email,
                    "Added to task",
                    $"You have been added to the task named '{volunteerTask.Name}'.");
            }
            await _context.SaveChangesAsync();
            await _mediator.PublishAsync(new VolunteerTaskAssignedToVolunteersNotification
            {
                VolunteerTaskId = message.VolunteerTaskId,
                NewlyAssignedVolunteers = new List<string> {message.UserId}
            });
        }
    }
}
