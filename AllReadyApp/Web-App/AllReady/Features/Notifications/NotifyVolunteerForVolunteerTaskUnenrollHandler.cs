using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AllReady.Configuration;
using MediatR;
using Microsoft.Extensions.Options;

namespace AllReady.Features.Notifications
{
    public class NotifyVolunteerForVolunteerTaskUnenrollHandler : IAsyncNotificationHandler<UserUnenrolled>
    {
        private readonly IMediator _mediator;
        private readonly IOptions<GeneralSettings> _options;

        public NotifyVolunteerForVolunteerTaskUnenrollHandler(IMediator mediator, IOptions<GeneralSettings> options)
        {
            _mediator = mediator;
            _options = options;
        }

        public async Task Handle(UserUnenrolled notification)
        {
            var volunteerTaskInfo = await _mediator.SendAsync(new VolunteerTaskDetailForNotificationQuery { VolunteerTaskId = notification.VolunteerTaskId, UserId = notification.UserId });

            if (volunteerTaskInfo == null)
            {
                return;
            }

            var emailRecipient = volunteerTaskInfo?.Volunteer?.Email;

            if (string.IsNullOrWhiteSpace(emailRecipient))
            {
                return;
            }

            var eventLink = $"View event: {_options.Value.SiteBaseUrl}Event/Details/{volunteerTaskInfo.EventId}";

            var message = new StringBuilder();
            message.AppendLine($"This is to confirm that you have elected to un-enroll from the following task:");
            message.AppendLine();
            message.AppendLine($"   Campaign: {volunteerTaskInfo.CampaignName}");
            message.AppendLine($"   Event: {volunteerTaskInfo.EventName} ({eventLink})");
            message.AppendLine($"   Task: {volunteerTaskInfo.VolunteerTaskName}");
            message.AppendLine();
            message.AppendLine("Thanks for letting us know that you will not be participating.");

            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    EmailMessage = message.ToString(),
                    HtmlMessage = message.ToString(),
                    EmailRecipients = new List<string> { emailRecipient },
                    Subject = "allReady Task Un-enrollment Confirmation"
                }
            };

            await _mediator.SendAsync(command);
        }
    }
}