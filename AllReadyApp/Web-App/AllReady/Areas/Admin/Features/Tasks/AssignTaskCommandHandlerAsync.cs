using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class AssignTaskCommandHandlerAsync : AsyncRequestHandler<AssignTaskCommandAsync>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        public AssignTaskCommandHandlerAsync(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        protected override async Task HandleCore(AssignTaskCommandAsync message)
        {
            var task = GetTask(message);
            var campaignEvent = task.Event;
            var taskSignups = new List<TaskSignup>();

            if (task != null)
            {
                //New Items, if not in collection add them, save that list for the pub-event
                foreach (var userId in message.UserIds)
                {
                    var taskSignup = task.AssignedVolunteers.SingleOrDefault(a => a.User.Id == userId);
                    if (taskSignup == null)
                    {
                        var user = _context.Users.Single(u => u.Id == userId);
                        taskSignup = new TaskSignup
                        {
                            Task = task,
                            User = user,
                            PreferredEmail = user.Email,
                            PreferredPhoneNumber = user.PhoneNumber,
                            AdditionalInfo = string.Empty,
                            Status = TaskStatus.Assigned.ToString(),
                            StatusDateTimeUtc = DateTime.UtcNow
                        };

                        task.AssignedVolunteers.Add(taskSignup);
                        taskSignups.Add(taskSignup);

                        // If the user has not already been signed up for the event, sign them up
                        if (campaignEvent.UsersSignedUp.All(acsu => acsu.User.Id != userId))
                        {
                            campaignEvent.UsersSignedUp.Add(new EventSignup
                            {
                                Event = campaignEvent,
                                User = user,
                                PreferredEmail = user.Email,
                                PreferredPhoneNumber = user.PhoneNumber,
                                AdditionalInfo = string.Empty,
                                SignupDateTime = DateTime.UtcNow
                            });
                        }
                    }
                }

                //Remove task signups where the the user id is not included in the current list of assigned user id's
                var taskSignupsToRemove = task.AssignedVolunteers.Where(taskSignup => message.UserIds.All(uid => uid != taskSignup.User.Id)).ToList();
                taskSignupsToRemove.ForEach(taskSignup => task.AssignedVolunteers.Remove(taskSignup));

                    // delete the event signups where the user is no longer signed up for any tasks
                    (from taskSignup in taskSignupsToRemove
                        where !campaignEvent.IsUserInAnyTask(taskSignup.User.Id)
                        select campaignEvent.UsersSignedUp.FirstOrDefault(u => u.User.Id == taskSignup.User.Id))
                        .ToList()
                        .ForEach(signup => campaignEvent.UsersSignedUp.Remove(signup));
            }
            await _context.SaveChangesAsync();

            // send all notifications to the queue
            var smsRecipients = new List<string>();
            var emailRecipients = new List<string>();

            // get all confirmed contact points for the broadcast
            smsRecipients.AddRange(taskSignups.Where(u => u.User.PhoneNumberConfirmed).Select(v => v.User.PhoneNumber));
            emailRecipients.AddRange(taskSignups.Where(u => u.User.EmailConfirmed).Select(v => v.User.Email));

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

            await _mediator.SendAsync(command);
        }

        private AllReadyTask GetTask(AssignTaskCommandAsync message)
        {
            var task = _context.Tasks
                .Include(t => t.Event).ThenInclude(a => a.UsersSignedUp)
                .SingleOrDefault(c => c.Id == message.TaskId);
            return task;
        }
    }
}
