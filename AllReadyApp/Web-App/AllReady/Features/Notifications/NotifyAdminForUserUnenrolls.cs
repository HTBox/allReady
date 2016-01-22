using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllReady.Areas.Admin.Features.Activities;
using AllReady.Features.Login;
using AllReady.Models;
using MediatR;
using Microsoft.Extensions.OptionsModel;

namespace AllReady.Features.Notifications
{
    public class NotifyAdminForUserUnenrolls : INotificationHandler<UserUnenrolls>
    {
        private readonly IMediator _bus;
        private readonly IOptions<GeneralSettings> _options;

        public NotifyAdminForUserUnenrolls(IMediator bus, IOptions<GeneralSettings> options)
        {
            _bus = bus;
            _options = options;
        }

        public void Handle(UserUnenrolls notification)
        {
            var model = _bus.Send(new UserUnenrollsNotificationQuery {ActivityId = notification.ActivityId});
            var campaignContact = model.CampaignContacts.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);

            if (string.IsNullOrWhiteSpace(campaignContact?.Contact.Email)) return;

            var signup = model.UsersSignedUp.FirstOrDefault(s => s.User.Id == notification.UserId);

            if (signup == null) return;

            var activityLink = $"View activity: {_options.Value.SiteBaseUrl}Admin/Activity/Details/{model.ActivityId}";
            var subject = $"A volunteer has unenrolled from {model.ActivityName}";

            var message = new StringBuilder();
            message.AppendLine($"A volunteer has unenrolled from an activity.");
            message.AppendLine($"   Campaign: {model.CampaignName}");
            message.AppendLine($"   Activity: {model.ActivityName} ({activityLink})");
            message.AppendLine($"   Volunteer: {signup.User.UserName} ({signup.User.Email})");
            message.AppendLine($"   Remaining/Required Volunteers: {model.UsersSignedUp.Count - 1}/{model.NumberOfVolunteersRequired}");
            message.AppendLine();

            var assignedTasks = model.Tasks.Where(t => t.AssignedVolunteers.Any(au => au.UserId == signup.User.Id)).ToList();
            if (assignedTasks.Count == 0)
            {
                message.AppendLine("This volunteer had not been assigned to any tasks.");
            }
            else
            {
                message.AppendLine("This volunteer had been assigned to the following tasks:");
                message.AppendLine  ("   Name             Description               Start Date           TaskLink");
                foreach (var task in assignedTasks)
                {
                    var taskLink = $"View task: {_options.Value.SiteBaseUrl}Admin/Task/Details/{task.Id}";
                    message.AppendFormat("   {0}{1}{2:d}{3}",
                        task.Name?.Substring(0, Math.Min(15, task.Name.Length)).PadRight(17, ' ') ?? "None".PadRight(17, ' '),
                        task.Description?.Substring(0, Math.Min(25, task.Description.Length)).PadRight(26, ' ') ?? "None".PadRight(26,' '),
                        task.StartDateTime?.Date.ToShortDateString().PadRight(21, ' ') ?? "".PadRight(21, ' '),
                        taskLink);
                }
            }

            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    EmailMessage = message.ToString(),
                    HtmlMessage = message.ToString(),
                    EmailRecipients = new List<string> { campaignContact.Contact.Email },
                    Subject = subject
                }
            };
            _bus.Send(command);
        }
    }
}