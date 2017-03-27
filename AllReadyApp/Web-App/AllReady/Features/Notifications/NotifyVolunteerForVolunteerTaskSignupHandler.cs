using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AllReady.Configuration;
using MediatR;
using Microsoft.Extensions.Options;

namespace AllReady.Features.Notifications
{
    public class NotifyVolunteerForVolunteerTaskSignupHandler : IAsyncNotificationHandler<VolunteerSignedUpNotification>
    {
        private readonly IMediator _mediator;
        private readonly IOptions<GeneralSettings> _options;

        public NotifyVolunteerForVolunteerTaskSignupHandler(IMediator mediator, IOptions<GeneralSettings> options)
        {
            _mediator = mediator;
            _options = options;
        }

        public async Task Handle(VolunteerSignedUpNotification notification)
        {
            var volunteerTaskInfo = await _mediator.SendAsync(new VolunteerTaskDetailForNotificationQuery { VolunteerTaskId = notification.VolunteerTaskId, UserId = notification.UserId });

            var emailRecipient = volunteerTaskInfo?.Volunteer.Email;

            if (string.IsNullOrWhiteSpace(emailRecipient))
            {
                return;
            }

            var eventLink = $"View event: {_options.Value.SiteBaseUrl}/Event/Details/{volunteerTaskInfo.EventId}";
            const string subject = "allReady Task Enrollment Confirmation";

            var message = new StringBuilder();
            message.AppendLine("This is to confirm that you have volunteered to participate in the following task:");
            message.AppendLine();
            message.AppendLine($"   Campaign: {volunteerTaskInfo.CampaignName}");
            message.AppendLine($"   Event: {volunteerTaskInfo.EventName} ({eventLink})");
            message.AppendLine($"   Task: {volunteerTaskInfo.VolunteerTaskName}");
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

            await _mediator.SendAsync(command);
        }
    }
}