using System;
using System.Collections.Generic;
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
                var notificationModel = await _mediator.SendAsync(new EventDetailForNotificationQueryAsync { EventId = notification.EventId, UserId = notification.UserId })
                    .ConfigureAwait(false);

                var campaignContact = notificationModel.CampaignContacts.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
                var adminEmail = campaignContact?.Contact.Email;

                if (string.IsNullOrWhiteSpace(adminEmail))
                {
                    return;
                }

                TaskSummaryModel taskModel = null;
                string remainingRequiredVolunteersPhrase;

                    taskModel = notificationModel.Tasks.FirstOrDefault(t => t.Id == notification.TaskIds[0]) ?? new TaskSummaryModel();
                    remainingRequiredVolunteersPhrase = $"{taskModel.NumberOfVolunteersSignedUp}/{taskModel.NumberOfVolunteersRequired}";

                var eventLink = $"View event: {_options.Value.SiteBaseUrl}Admin/Event/Details/{notificationModel.EventId}";
                var subject = $"A volunteer has unenrolled from a task";

                var message = new StringBuilder();
                message.AppendLine($"A volunteer has unenrolled from a task.");
                message.AppendLine($"   Volunteer: {notificationModel.Volunteer.Name} ({notificationModel.Volunteer.Email})");
                message.AppendLine($"   Campaign: {notificationModel.CampaignName}");
                message.AppendLine($"   Event: {notificationModel.EventName} ({eventLink})");
                    message.AppendLine($"   Task: {taskModel.Name} ({$"View task: {_options.Value.SiteBaseUrl}Admin/Task/Details/{taskModel.Id}"})");
                    message.AppendLine($"   Task Start Date: {taskModel.StartDateTime?.Date.ToShortDateString()}");
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

                await _mediator.SendAsync(command).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception encountered: message={e.Message}, innerException={e.InnerException}, stacktrace={e.StackTrace}");
            }
        }
    }
}
