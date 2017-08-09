using AllReady.Features.Notifications;
using MediatR;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class SendEventManagerInviteHandler : IAsyncNotificationHandler<SendEventManagerInvite>
    {
        private IMediator _mediator;

        public SendEventManagerInviteHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(SendEventManagerInvite notification)
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

        private string BuildPlainTextMessage(SendEventManagerInvite notification)
        {
            var plainTextMessage = new StringBuilder();
            plainTextMessage.AppendLine($"Event manager invite for event {notification.EventName}");
            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine($"{notification.SenderName} has invited you have been invited you to become event manager for the event {notification.EventName}.");
            plainTextMessage.AppendLine("To accept go to the following URL: ");
            plainTextMessage.AppendLine(notification.AcceptUrl);
            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine("To decline go to the following Url: ");
            plainTextMessage.AppendLine(notification.DeclineUrl);

            return plainTextMessage.ToString();
        }

        private string BuildHtmlMessage(SendEventManagerInvite notification)
        {
            var htmlTextMessage = new StringBuilder();
            htmlTextMessage.AppendLine($"Event manager invite for event {notification.EventName}");
            htmlTextMessage.AppendLine();
            htmlTextMessage.AppendLine($"{notification.SenderName} has invited you have been invited you to become event manager for the event {notification.EventName}.");
            htmlTextMessage.AppendLine($"To accept <a href=\"{notification.AcceptUrl}\">click here</a>.");
            htmlTextMessage.AppendLine($"To decline <a href=\"{notification.DeclineUrl}\">click here</a>.");

            return htmlTextMessage.ToString();
        }
    }
}
