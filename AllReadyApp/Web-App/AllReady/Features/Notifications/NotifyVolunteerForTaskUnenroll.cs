using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;

namespace AllReady.Features.Notifications
{
    public class NotifyVolunteerForTaskUnenroll : IAsyncNotificationHandler<UserUnenrolls>
    {
        private readonly IMediator _mediator;
        private readonly IOptions<GeneralSettings> _options;

        public NotifyVolunteerForTaskUnenroll(IMediator mediator, IOptions<GeneralSettings> options)
        {
            _mediator = mediator;
            _options = options;
        }

        public async Task Handle(UserUnenrolls notification)
        {
            var taskInfo = await _mediator.SendAsync(new TaskDetailForNotificationQueryAsync { TaskId = notification.TaskId, UserId = notification.UserId })
                .ConfigureAwait(false);

            if (taskInfo == null)
            {
                return;
            }

            var emailRecipient = taskInfo?.Volunteer?.Email;

            if (string.IsNullOrWhiteSpace(emailRecipient))
            {
                return;
            }

            var eventLink = $"View event: {_options.Value.SiteBaseUrl}Event/Details/{taskInfo.EventId}";

            var message = new StringBuilder();
            message.AppendLine($"This is to confirm that you have elected to un-enroll from the following task:");
            message.AppendLine();
            message.AppendLine($"   Campaign: {taskInfo.CampaignName}");
            message.AppendLine($"   Event: {taskInfo.EventName} ({eventLink})");
            message.AppendLine($"   Task: {taskInfo.TaskName}");
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

            await _mediator.SendAsync(command).ConfigureAwait(false);
        }
    }
}