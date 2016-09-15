using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AllReady.Features.Notifications
{
    public class NotifyAdminForSignup : IAsyncNotificationHandler<VolunteerSignupNotification>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;
        private readonly IOptions<GeneralSettings> _options;
        private readonly ILogger<NotifyAdminForUserUnenrolls> _logger;

        public NotifyAdminForSignup(AllReadyContext context, IMediator mediator, IOptions<GeneralSettings> options, ILogger<NotifyAdminForUserUnenrolls> logger)
        {
            _context = context;
            _mediator = mediator;
            _options = options;
            _logger = logger;
        }

        public async Task Handle(VolunteerSignupNotification notification)
        {
            // don't let problem with notification keep us from continuing
            try
            {
                var taskInfo = await _mediator.SendAsync(new TaskDetailForNotificationQueryAsync { TaskId = notification.TaskId, UserId = notification.UserId })
                     .ConfigureAwait(false);

                var campaignContact = taskInfo.CampaignContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
                var adminEmail = campaignContact?.Contact.Email;
                if (string.IsNullOrWhiteSpace(adminEmail))
                {
                    return;
                }

                var eventLink = $"View event: http://{_options.Value.SiteBaseUrl}/Admin/Event/Details/{taskInfo.EventId}";

                var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == notification.TaskId).ConfigureAwait(false);
                if (task == null)
                {
                    return;
                }

                var taskLink = $"View task: http://{_options.Value.SiteBaseUrl}/Admin/task/Details/{task.Id}";
                var taskSignup = task.AssignedVolunteers.FirstOrDefault(t => t.User.Id == taskInfo.Volunteer.Id);

                //set for task signup
                //var volunteerEmail = !string.IsNullOrWhiteSpace(taskSignup?.PreferredEmail) ? taskSignup.PreferredEmail : taskInfo.Volunteer.Email;
                //var volunteerPhoneNumber = !string.IsNullOrWhiteSpace(taskSignup?.PreferredPhoneNumber) ? taskSignup.PreferredPhoneNumber : taskInfo.Volunteer.PhoneNumber;
                var volunteerEmail = taskInfo.Volunteer.Email;
                var volunteerPhoneNumber = taskInfo.Volunteer.PhoneNumber;
                var volunteerComments = !string.IsNullOrWhiteSpace(taskSignup?.AdditionalInfo) ? taskSignup.AdditionalInfo : string.Empty;
                var remainingRequiredVolunteersPhrase = $"{task.NumberOfUsersSignedUp}/{task.NumberOfVolunteersRequired}";
                const string typeOfSignupPhrase = "a task";

                var subject = $"A volunteer has signed up for {typeOfSignupPhrase}";

                var message = new StringBuilder();
                message.AppendLine($"A volunteer has signed up for {typeOfSignupPhrase}:");
                message.AppendLine();
                message.AppendLine($"   Campaign: {taskInfo.CampaignName}");
                message.AppendLine($"   Event: {taskInfo.EventName} ({eventLink})");
                message.AppendLine($"   Task: {task.Name} ({taskLink})");
                message.AppendLine($"   Remaining/Required Volunteers: {remainingRequiredVolunteersPhrase}");
                message.AppendLine();
                message.AppendLine($"   Volunteer Name: {taskInfo.Volunteer.Name}");
                message.AppendLine($"   Volunteer Email: {volunteerEmail}");
                message.AppendLine($"   Volunteer PhoneNumber: {volunteerPhoneNumber}");
                message.AppendLine($"   Volunteer Comments: {volunteerComments}");
                message.AppendLine();
                message.AppendLine(GetTaskSkillsInfo(task, taskInfo.Volunteer));

                Debug.WriteLine(adminEmail);
                Debug.WriteLine(subject);
                Debug.WriteLine(message.ToString());

                if (!string.IsNullOrWhiteSpace(adminEmail))
                {
                    var command = new NotifyVolunteersCommand
                    {
                        ViewModel = new NotifyVolunteersViewModel
                        {
                            EmailMessage = message.ToString(),
                            HtmlMessage = message.ToString(),
                            EmailRecipients = new List<string> { adminEmail },
                            Subject = subject
                        }
                    };

                    await _mediator.SendAsync(command).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception encountered: message={e.Message}, innerException={e.InnerException}, stacktrace={e.StackTrace}");
            }
        }

        private static string GetTaskSkillsInfo(AllReadyTask task, ApplicationUser volunteer)
        {
            var result = new StringBuilder();
            if (task.RequiredSkills.Count == 0)
            {
                return result.ToString();
            }
            result.AppendLine("   Skills Required:");
            foreach (var skill in task.RequiredSkills)
            {
                var userMatch = volunteer.AssociatedSkills.Any(vs => vs.SkillId == skill.SkillId);
                result.AppendLine($"      {skill.Skill.Name} {(userMatch ? "(match)" : string.Empty)}");
            }
            return result.ToString();
        }
    }
}
