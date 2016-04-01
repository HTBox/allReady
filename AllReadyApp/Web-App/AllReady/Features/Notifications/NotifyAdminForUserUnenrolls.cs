using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;

namespace AllReady.Features.Notifications
{
    public class NotifyAdminForUserUnenrolls : IAsyncNotificationHandler<UserUnenrolls>
    {
        private readonly IMediator _mediator;
        private readonly IOptions<GeneralSettings> _options;
        private readonly ILogger<NotifyAdminForUserUnenrolls> _logger;

        public NotifyAdminForUserUnenrolls(IMediator mediator, IOptions<GeneralSettings> options, ILogger<NotifyAdminForUserUnenrolls> logger)
        {
            _mediator = mediator;
            _options = options;
            _logger = logger;
        }

        public async Task Handle(UserUnenrolls notification)
        {
            // don't let problem with notification keep us from continuing
            try
            {
                var notificationModel = await _mediator.SendAsync(new ActivityDetailForNotificationQueryAsync { ActivityId = notification.ActivityId, UserId = notification.UserId });

                var campaignContact = notificationModel.CampaignContacts.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
                var adminEmail = campaignContact?.Contact.Email;

                if (string.IsNullOrWhiteSpace(adminEmail))
                {
                    return;
                }

                TaskSummaryModel taskModel = null;
                string remainingRequiredVolunteersPhrase;
                var isActivityUnenroll = (notificationModel.ActivityType == ActivityTypes.ActivityManaged);

                if (isActivityUnenroll)
                {
                    remainingRequiredVolunteersPhrase = $"{notificationModel.UsersSignedUp.Count}/{notificationModel.NumberOfVolunteersRequired}";
                }
                else
                {
                    taskModel = notificationModel.Tasks.FirstOrDefault(t => t.Id == notification.TaskIds[0]) ?? new TaskSummaryModel();
                    remainingRequiredVolunteersPhrase = $"{taskModel.NumberOfVolunteersSignedUp}/{taskModel.NumberOfVolunteersRequired}";
                }
            

                var activityLink = $"View activity: {_options.Value.SiteBaseUrl}Admin/Activity/Details/{notificationModel.ActivityId}";
                var subject = $"A volunteer has unenrolled from {(isActivityUnenroll ? "an activity" : "a task")}";

                var message = new StringBuilder();
                message.AppendLine($"A volunteer has unenrolled from {(isActivityUnenroll ? "an activity" : "a task")}.");
                message.AppendLine($"   Volunteer: {notificationModel.Volunteer.Name} ({notificationModel.Volunteer.Email})");
                message.AppendLine($"   Campaign: {notificationModel.CampaignName}");
                message.AppendLine($"   Activity: {notificationModel.ActivityName} ({activityLink})");
                if (!isActivityUnenroll)
                {
                    message.AppendLine($"   Task: {taskModel.Name} ({$"View task: {_options.Value.SiteBaseUrl}Admin/Task/Details/{taskModel.Id}"})");
                    message.AppendLine($"   Task Start Date: {taskModel.StartDateTime?.Date.ToShortDateString()}");
                }
                message.AppendLine($"   Remaining/Required Volunteers: {remainingRequiredVolunteersPhrase}");
                message.AppendLine();


                if (isActivityUnenroll)
                {
                    var assignedTasks = notificationModel.Tasks.Where(t => t.AssignedVolunteers.Any(au => au.UserId == notificationModel.Volunteer.Id)).ToList();
                    if (assignedTasks.Count == 0)
                    {
                        message.AppendLine("This volunteer had not been assigned to any tasks.");
                    }
                    else
                    {
                        message.AppendLine("This volunteer had been assigned to the following tasks:");
                        message.AppendLine("   Name             Description               Start Date           TaskLink");
                        foreach (var assignedTask in assignedTasks)
                        {
                            var taskLink = $"View task: {_options.Value.SiteBaseUrl}Admin/Task/Details/{assignedTask.Id}";
                            message.AppendFormat("   {0}{1}{2:d}{3}",
                                assignedTask.Name?.Substring(0, Math.Min(15, assignedTask.Name.Length)).PadRight(17, ' ') ??
                                "None".PadRight(17, ' '),
                                assignedTask.Description?.Substring(0, Math.Min(25, assignedTask.Description.Length)).PadRight(26, ' ') ??
                                "None".PadRight(26, ' '),
                                assignedTask.StartDateTime?.Date.ToShortDateString().PadRight(21, ' ') ?? "".PadRight(21, ' '),
                                taskLink);
                        }
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
            await _mediator.SendAsync(command).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception encountered: message={e.Message}, innerException={e.InnerException}, stacktrace={e.StackTrace}");
            }
        }
    }
}