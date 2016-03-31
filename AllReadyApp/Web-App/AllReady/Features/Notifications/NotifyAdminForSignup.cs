using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;

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
                var volunteer = await _context.Users.SingleAsync(u => u.Id == notification.UserId);

                var activity = await _context.Activities
                    .Include(a => a.RequiredSkills).ThenInclude(r => r.Skill)
                    .SingleAsync(a => a.Id == notification.ActivityId);

                var activitySignup = activity.UsersSignedUp.FirstOrDefault(a => a.User.Id == notification.UserId);

                var campaign = await _context.Campaigns
                    .Include(c => c.CampaignContacts).ThenInclude(cc => cc.Contact)
                    .SingleOrDefaultAsync(c => c.Id == activity.CampaignId);

                var campaignContact = campaign.CampaignContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
                var adminEmail = campaignContact?.Contact.Email;
                if (string.IsNullOrWhiteSpace(adminEmail))
                {
                    return;
                }

                var isActivitySignup = (activity.ActivityType == ActivityTypes.ActivityManaged);
                var activityLink = $"View activity: http://{_options.Value.SiteBaseUrl}/Admin/Activity/Details/{activity.Id}";

                AllReadyTask task = null;
                string taskLink = null;
                TaskSignup taskSignup = null;

                if (!isActivitySignup)
                {
                    task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == notification.TaskId);
                    if (task == null)
                    {
                        return;
                    }
                    taskLink = $"View task: http://{_options.Value.SiteBaseUrl}/Admin/task/Details/{task.Id}";
                    taskSignup = task.AssignedVolunteers.FirstOrDefault(t => t.User.Id == volunteer.Id); 
                }

                //set defaults for activity signup
                var volunteerEmail = !string.IsNullOrWhiteSpace(activitySignup?.PreferredEmail) ? activitySignup.PreferredEmail : volunteer.Email;
                var volunteerPhoneNumber = !string.IsNullOrWhiteSpace(activitySignup?.PreferredPhoneNumber) ? activitySignup.PreferredPhoneNumber : volunteer.PhoneNumber;
                var volunteerComments = activitySignup?.AdditionalInfo ?? string.Empty;
                var remainingRequiredVolunteersPhrase = $"{activity.NumberOfUsersSignedUp}/{activity.NumberOfVolunteersRequired}";
                var typeOfSignupPhrase = "an activity";

                if (activity.ActivityType != ActivityTypes.ActivityManaged)
                {
                    //set for task signup
                    volunteerEmail = !string.IsNullOrWhiteSpace(taskSignup?.PreferredEmail) ? taskSignup.PreferredEmail : volunteerEmail;
                    volunteerPhoneNumber = !string.IsNullOrWhiteSpace(taskSignup?.PreferredPhoneNumber) ? taskSignup.PreferredPhoneNumber : volunteerPhoneNumber;
                    volunteerComments = !string.IsNullOrWhiteSpace(taskSignup?.AdditionalInfo) ? taskSignup.AdditionalInfo : volunteerComments;
                    remainingRequiredVolunteersPhrase = $"{task.NumberOfUsersSignedUp}/{task.NumberOfVolunteersRequired}";
                    typeOfSignupPhrase = "a task";
                }

                var subject = $"A volunteer has signed up for {typeOfSignupPhrase}";

                var message = new StringBuilder();
                message.AppendLine($"A volunteer has signed up for {typeOfSignupPhrase}:");
                message.AppendLine();
                message.AppendLine($"   Campaign: {campaign.Name}");
                message.AppendLine($"   Activity: {activity.Name} ({activityLink})");
                if (!isActivitySignup)
                {
                    message.AppendLine($"   Task: {task.Name} ({taskLink})");
                }
                message.AppendLine($"   Remaining/Required Volunteers: {remainingRequiredVolunteersPhrase}");
                message.AppendLine();
                message.AppendLine($"   Volunteer Name: {volunteer.Name}");
                message.AppendLine($"   Volunteer Email: {volunteerEmail}");
                message.AppendLine($"   Volunteer PhoneNumber: {volunteerPhoneNumber}");
                message.AppendLine($"   Volunteer Comments: {volunteerComments}");
                message.AppendLine();
                message.AppendLine(isActivitySignup ? GetActivitySkillsInfo(activity, volunteer) : GetTaskSkillsInfo(task, volunteer));

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

        private string GetActivitySkillsInfo(Models.Activity activity, ApplicationUser volunteer)
        {
            var result = new StringBuilder();
            if (activity.RequiredSkills.Count == 0) return result.ToString();
            result.AppendLine("   Skills Required:");
            foreach (var skill in activity.RequiredSkills)
            {
                var userMatch = volunteer.AssociatedSkills.Any(vs => vs.SkillId == skill.SkillId);
                result.AppendLine($"      {skill.Skill.Name} {(userMatch ? "(match)" : string.Empty)}");
            }
            return result.ToString();
        }

        private string GetTaskSkillsInfo(AllReadyTask task, ApplicationUser volunteer)
        {
            var result = new StringBuilder();
            if (task.RequiredSkills.Count == 0) return result.ToString();
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