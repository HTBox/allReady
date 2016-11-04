﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
                var taskModel = await _mediator.SendAsync(new TaskDetailForNotificationQuery { TaskId = notification.TaskId, UserId = notification.UserId })
                    .ConfigureAwait(false);

                var campaignContact = taskModel.CampaignContacts.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary);
                var adminEmail = campaignContact?.Contact.Email;

                if (string.IsNullOrWhiteSpace(adminEmail))
                {
                    return;
                }
                string remainingRequiredVolunteersPhrase = $"{taskModel.NumberOfVolunteersSignedUp}/{taskModel.NumberOfVolunteersRequired}";

                var eventLink = $"View event: {_options.Value.SiteBaseUrl}Admin/Event/Details/{taskModel.EventId}";
                var subject = $"A volunteer has unenrolled from a task";

                var message = new StringBuilder();
                message.AppendLine($"A volunteer has unenrolled from a task.");
                message.AppendLine($"   Volunteer: {taskModel.Volunteer.Name} ({taskModel.Volunteer.Email})");
                message.AppendLine($"   Campaign: {taskModel.CampaignName}");
                message.AppendLine($"   Event: {taskModel.EventName} ({eventLink})");
                message.AppendLine($"   Task: {taskModel.TaskName} (View task: {_options.Value.SiteBaseUrl}Admin/Task/Details/{taskModel.TaskId})");
                message.AppendLine($"   Task Start Date: {taskModel.TaskStartDate.Date.ToShortDateString()}");
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
