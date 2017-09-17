using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllReady.Configuration;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AllReady.Features.Notifications
{
    public class NotifyAdminForSignupHandler : IAsyncNotificationHandler<VolunteerSignedUpNotification>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;
        private readonly IOptions<GeneralSettings> _options;
        private readonly ILogger<NotifyAdminForUserUnenrollsHandler> _logger;

        public NotifyAdminForSignupHandler(AllReadyContext context, IMediator mediator, IOptions<GeneralSettings> options, ILogger<NotifyAdminForUserUnenrollsHandler> logger)
        {
            _context = context;
            _mediator = mediator;
            _options = options;
            _logger = logger;
        }

        public async Task Handle(VolunteerSignedUpNotification notification)
        {
            // don't let problem with notification keep us from continuing
            try
            {
                var volunteerTaskInfo = await _mediator.SendAsync(new VolunteerTaskDetailForNotificationQuery { VolunteerTaskId = notification.VolunteerTaskId, UserId = notification.UserId });

                var campaignContact = volunteerTaskInfo.CampaignContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
                var adminEmail = campaignContact?.Contact?.Email;
                if (string.IsNullOrWhiteSpace(adminEmail))
                {
                    return;
                }

                var eventLink = $"View event: http://{_options.Value.SiteBaseUrl}/Admin/Event/Details/{volunteerTaskInfo.EventId}";

                var volunteerTask = await _context.VolunteerTasks.SingleOrDefaultAsync(t => t.Id == notification.VolunteerTaskId);
                if (volunteerTask == null)
                {
                    return;
                }

                var volunteerTaskLink = $"View task: http://{_options.Value.SiteBaseUrl}/Admin/VolunteerTask/Details/{volunteerTask.Id}";
                var volunteerTaskSignup = volunteerTask.AssignedVolunteers.FirstOrDefault(t => t.User.Id == volunteerTaskInfo.Volunteer.Id);

                //set for task signup
                var volunteerEmail = volunteerTaskInfo.Volunteer.Email;
                var volunteerPhoneNumber = volunteerTaskInfo.Volunteer.PhoneNumber;
                var volunteerComments = !string.IsNullOrWhiteSpace(volunteerTaskSignup?.AdditionalInfo) ? volunteerTaskSignup.AdditionalInfo : string.Empty;
                var remainingRequiredVolunteersPhrase = $"{volunteerTask.NumberOfUsersSignedUp}/{volunteerTask.NumberOfVolunteersRequired}";
                const string typeOfSignupPhrase = "a task";

                var subject = $"A volunteer has signed up for {typeOfSignupPhrase}";

                var message = new StringBuilder();
                message.AppendLine($"A volunteer has signed up for {typeOfSignupPhrase}:");
                message.AppendLine();
                message.AppendLine($"   Campaign: {volunteerTaskInfo.CampaignName}");
                message.AppendLine($"   Event: {volunteerTaskInfo.EventName} ({eventLink})");
                message.AppendLine($"   Task: {volunteerTask.Name} ({volunteerTaskLink})");
                message.AppendLine($"   Remaining/Required Volunteers: {remainingRequiredVolunteersPhrase}");
                message.AppendLine();
                message.AppendLine($"   Volunteer Name: {volunteerTaskInfo.Volunteer.Name}");
                message.AppendLine($"   Volunteer Email: {volunteerEmail}");
                message.AppendLine($"   Volunteer PhoneNumber: {volunteerPhoneNumber}");
                message.AppendLine($"   Volunteer Comments: {volunteerComments}");
                message.AppendLine();
                message.AppendLine(GetVolunteerTaskSkillsInfo(volunteerTask, volunteerTaskInfo.Volunteer));

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

                await _mediator.SendAsync(command);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception encountered: message={e.Message}, innerException={e.InnerException}, stacktrace={e.StackTrace}");
            }
        }

        private static string GetVolunteerTaskSkillsInfo(VolunteerTask volunteerTask, ApplicationUser volunteer)
        {
            var result = new StringBuilder();
            if (volunteerTask.RequiredSkills.Count == 0)
            {
                return result.ToString();
            }
            result.AppendLine("   Skills Required:");
            foreach (var skill in volunteerTask.RequiredSkills)
            {
                var userMatch = volunteer.AssociatedSkills.Any(vs => vs.SkillId == skill.SkillId);
                result.AppendLine($"      {skill.Skill.Name} {(userMatch ? "(match)" : string.Empty)}");
            }
            return result.ToString();
        }
    }
}
