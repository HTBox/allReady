using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;

namespace AllReady.Features.Notifications
{
    public class NotifyVolunteerForTaskSignup : IAsyncNotificationHandler<VolunteerSignedUpNotification>
    {
        private readonly IMediator _mediator;
        private readonly IOptions<GeneralSettings> _options;

        public NotifyVolunteerForTaskSignup(IMediator mediator, IOptions<GeneralSettings> options)
        {
            _mediator = mediator;
            _options = options;
        }

        public async Task Handle(VolunteerSignedUpNotification notification)
        {
            var taskInfo = await _mediator.SendAsync(new TaskDetailForNotificationQueryAsync { TaskId = notification.TaskId, UserId = notification.UserId })
                .ConfigureAwait(false);

            var emailRecipient = taskInfo?.Volunteer.Email;

            if (string.IsNullOrWhiteSpace(emailRecipient))
            {
                return;
            }

            var eventLink = $"View event: {_options.Value.SiteBaseUrl}/Event/Details/{taskInfo.EventId}";
            const string subject = "allReady Task Enrollment Confirmation";

            var message = new StringBuilder();
            message.AppendLine("This is to confirm that you have volunteered to participate in the following task:");
            message.AppendLine();
            message.AppendLine($"   Campaign: {taskInfo.CampaignName}");
            message.AppendLine($"   Event: {taskInfo.EventName} ({eventLink})");
            message.AppendLine($"   Task: {taskInfo.TaskName}");
            message.AppendLine();
            message.AppendLine("Thanks for volunteering. Your help is appreciated.");

            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    EmailMessage = message.ToString(),
                    HtmlMessage = message.ToString(),
                    EmailRecipients = new List<string> { emailRecipient},
                    Subject = subject
                }
            };

            await _mediator.SendAsync(command).ConfigureAwait(false);
        }
    }
}