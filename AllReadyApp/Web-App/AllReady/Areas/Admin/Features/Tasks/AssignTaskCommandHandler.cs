﻿using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class AssignTaskCommandHandler : RequestHandler<AssignTaskCommand>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _bus;

        public AssignTaskCommandHandler(AllReadyContext context, IMediator bus)
        {
            _context = context;
            _bus = bus;
        }

        protected override void HandleCore(AssignTaskCommand message)
        {
            var task = _context.Tasks.SingleOrDefault(c => c.Id == message.TaskId);
            var newVolunteers = new List<TaskSignup>();
            if (task != null)
            {
                //New Items, if not in collection add them, save that list for the pub-event
                foreach (var userId in message.UserIds)
                {
                    var av = task.AssignedVolunteers.SingleOrDefault(a => a.User.Id == userId);
                    if (av == null)
                    {
                        var volunteerUser = _context.Users.Single(u => u.Id == userId);
                        av = new TaskSignup
                        {
                            Task = task,
                            User = volunteerUser,
                            Status = TaskStatus.Assigned.ToString(),
                            StatusDateTimeUtc = DateTime.UtcNow
                        };
                    }
                    task.AssignedVolunteers.Add(av);
                    newVolunteers.Add(av);
                }
                //Remove Items Not there, All Volunteers should be in task.AssignedVolunteers
                var removedVolunteers = new List<TaskSignup>();
                foreach (var vol in task.AssignedVolunteers)
                {
                    if (!message.UserIds.Any(uid => uid == vol.User.Id))
                    {
                        removedVolunteers.Add(vol);
                    }
                }                
                foreach (var vol in removedVolunteers)
                {
                    task.AssignedVolunteers.Remove(vol);
                }
            }
            _context.SaveChanges();

            // send all notifications to the queue
            var smsRecipients = new List<string>();
            var emailRecipients = new List<string>();

            // get all confirmed contact points for the broadcast
            smsRecipients.AddRange(newVolunteers.Where(u => u.User.PhoneNumberConfirmed).Select(v => v.User.PhoneNumber));
            emailRecipients.AddRange(newVolunteers.Where(u => u.User.EmailConfirmed).Select(v => v.User.Email));

            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    SmsMessage = "You've been assigned a task from AllReady.",
                    SmsRecipients = smsRecipients,
                    EmailMessage = "You've been assigned a task from AllReady.",
                    HtmlMessage = "You've been assigned a task from AllReady.",
                    EmailRecipients = emailRecipients,
                    Subject = "You've been assigned a task from AllReady."
                }
            };

            _bus.Send(command);

        }
    }
}
