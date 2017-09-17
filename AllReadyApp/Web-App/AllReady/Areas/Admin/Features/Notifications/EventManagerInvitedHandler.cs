using AllReady.Features.Notifications;
using MediatR;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class EventManagerInvitedHandler : IAsyncNotificationHandler<EventManagerInvited>
    {
        private IMediator _mediator;

        public EventManagerInvitedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(EventManagerInvited notification)
        {
            var plainTextMessage = BuildPlainTextMessage(notification);
            var htmlMessage = BuildHtmlMessage(notification);

            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    EmailMessage = plainTextMessage.ToString(),
                    EmailRecipients = new List<string> { notification.InviteeEmail },
                    HtmlMessage = htmlMessage,
                    Subject = "Event manager invite"
                }
            };

            await _mediator.SendAsync(command);
        }

        private string BuildPlainTextMessage(EventManagerInvited notification)
        {
            var plainTextMessage = new StringBuilder();
            plainTextMessage.AppendLine($"Event manager invite for event {notification.EventName}");
            plainTextMessage.AppendLine();
            plainTextMessage.Append($"{notification.SenderName} has invited you to become event manager for the event {notification.EventName}");

            if (!string.IsNullOrWhiteSpace(notification.Message))
            {
                plainTextMessage.Append(" with the following message:");
                plainTextMessage.AppendLine();
                plainTextMessage.Append(notification.Message);
                plainTextMessage.AppendLine();
            }

            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine("To accept go to the following URL: ");
            plainTextMessage.AppendLine(notification.AcceptUrl);
            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine("To decline go to the following Url: ");
            plainTextMessage.AppendLine(notification.DeclineUrl);

            if (!notification.IsInviteeRegistered)
            {
                plainTextMessage.AppendLine();
                plainTextMessage.AppendLine("Before accepting the invite you need to register with Allready by clicking at this link: ");
                plainTextMessage.AppendLine(notification.RegisterUrl);
            }

            return plainTextMessage.ToString();
        }

        private string BuildHtmlMessage(EventManagerInvited notification)
        {
            var htmlTextMessage = new StringBuilder();
            htmlTextMessage.AppendLine($"Event manager invite for event {notification.EventName}");
            htmlTextMessage.AppendLine();
            htmlTextMessage.Append($"{notification.SenderName} has invited you to become event manager for the event {notification.EventName}");

            if (!string.IsNullOrWhiteSpace(notification.Message))
            {
                htmlTextMessage.Append(" with the following message:");
                htmlTextMessage.AppendLine();
                htmlTextMessage.AppendLine();
                htmlTextMessage.Append(notification.Message);
                htmlTextMessage.AppendLine();
                htmlTextMessage.AppendLine();
            }

            htmlTextMessage.AppendLine($"To accept <a href=\"{notification.AcceptUrl}\">click here</a>.");
            htmlTextMessage.AppendLine($"To decline <a href=\"{notification.DeclineUrl}\">click here</a>.");

            if (!notification.IsInviteeRegistered)
            {
                htmlTextMessage.AppendLine();
                htmlTextMessage.AppendLine($"Before accepting the invite you need to <a href=\"{notification.RegisterUrl}\">register with Allready</a>.");
            }

            return htmlTextMessage.ToString();
        }
    }
}
