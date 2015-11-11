using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using Microsoft.Framework.OptionsModel;

namespace AllReady.Features.Notifications
{
    public class NotifyAdminForTaskSignupStatusChange : INotificationHandler<TaskSignupStatusChanged>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _bus;
        private readonly IOptions<GeneralSettings> _options;

        public NotifyAdminForTaskSignupStatusChange(AllReadyContext context, IMediator bus, IOptions<GeneralSettings> options)
        {
            _context = context;
            _bus = bus;
            _options = options;
        }

        public void Handle(TaskSignupStatusChanged notification)
        {
            var taskSignup = _context.TaskSignups
                .Include(ts => ts.Task)
                    .ThenInclude(t => t.Activity).ThenInclude(a => a.Organizer)
                .Include(ts => ts.Task)
                    .ThenInclude(t => t.Activity).ThenInclude(a => a.Campaign).ThenInclude(c => c.Organizer)
                .Include(ts => ts.User)
                .Single(ts => ts.Id == notification.SignupId);
            var volunteer = taskSignup.User;
            var task = taskSignup.Task;
            var activity = task.Activity;
            var campaign = activity.Campaign;

            //Prefer activity organizer email; fallback to campaign organizer email
            var adminEmail = taskSignup.Task.Activity.Organizer?.Email;
            if (string.IsNullOrWhiteSpace(adminEmail))
            {
                adminEmail = taskSignup.Task.Activity.Campaign.Organizer?.Email;
            }
            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                var link = $"View activity: http://{_options.Value.SiteBaseUrl}/Admin/Task/Details/{taskSignup.Task.Id}";
                var subject = $"Task status changed ({taskSignup.Status}) for volunteer {volunteer.Name ?? volunteer.Email}";
                var command = new NotifyVolunteersCommand
                {
                    ViewModel = new NotifyVolunteersViewModel
                    {
                        EmailMessage = $@"A volunteer's status has changed for a task.

Volunteer: {volunteer.Name} ({volunteer.Email})
New status: {taskSignup.Status}
Task: {task.Name} {link}
Activity: {activity.Name}
Campaign: {campaign.Name}",
                        EmailRecipients = new List<string> { adminEmail },
                        Subject = subject
                    }
                };

                _bus.Send(command);
            }
        }
    }
}
