using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllReady.Configuration;
using AllReady.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AllReady.Features.Notifications
{
    public class NotifyAdminForUserUnenrollsHandler : IAsyncNotificationHandler<UserUnenrolled>
    {
        private readonly IMediator _mediator;
        private readonly IOptions<GeneralSettings> _options;
        private readonly ILogger<NotifyAdminForUserUnenrollsHandler> _logger;

        public NotifyAdminForUserUnenrollsHandler(IMediator mediator, IOptions<GeneralSettings> options, ILogger<NotifyAdminForUserUnenrollsHandler> logger)
        {
            _mediator = mediator;
            _options = options;
            _logger = logger;
        }

        public async Task Handle(UserUnenrolled notification)
        {
            // don't let problem with notification keep us from continuing
            try
            {
                var volunteerTaskModel = await _mediator.SendAsync(new VolunteerTaskDetailForNotificationQuery { VolunteerTaskId = notification.VolunteerTaskId, UserId = notification.UserId });

                var campaignContact = volunteerTaskModel.CampaignContacts.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
                var adminEmail = campaignContact?.Contact.Email;

                if (string.IsNullOrWhiteSpace(adminEmail))
                {
                    return;
                }
                string remainingRequiredVolunteersPhrase = $"{volunteerTaskModel.NumberOfVolunteersSignedUp}/{volunteerTaskModel.NumberOfVolunteersRequired}";

                var eventLink = $"View event: {_options.Value.SiteBaseUrl}Admin/Event/Details/{volunteerTaskModel.EventId}";
                var subject = $"A volunteer has unenrolled from a task";

                var message = new StringBuilder();
                message.AppendLine($"A volunteer has unenrolled from a task.");
                message.AppendLine($"   Volunteer: {volunteerTaskModel.Volunteer.Name} ({volunteerTaskModel.Volunteer.Email})");
                message.AppendLine($"   Campaign: {volunteerTaskModel.CampaignName}");
                message.AppendLine($"   Event: {volunteerTaskModel.EventName} ({eventLink})");
                message.AppendLine($"   Task: {volunteerTaskModel.VolunteerTaskName} (View task: {_options.Value.SiteBaseUrl}Admin/VolunteerTask/Details/{volunteerTaskModel.VolunteerTaskId})");
                message.AppendLine($"   Task Start Date: {volunteerTaskModel.VolunteerTaskStartDate.Date:d}");
                message.AppendLine($"   Remaining/Required Volunteers: {remainingRequiredVolunteersPhrase}");
                message.AppendLine();

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

                await _mediator.SendAsync(command);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception encountered: message={e.Message}, innerException={e.InnerException}, stacktrace={e.StackTrace}");
            }
        }
    }
}
