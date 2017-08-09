using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using System.Text;
using AllReady.Features.Notifications;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class SendCampaignManagerInviteHandler : IAsyncNotificationHandler<SendCampaignManagerInvite>
    {
        private IMediator _mediator;

        public SendCampaignManagerInviteHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(SendCampaignManagerInvite notification)
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
                    Subject = "Campaign manager invite"
                }
            };

            await _mediator.SendAsync(command);
        }

        private string BuildPlainTextMessage(SendCampaignManagerInvite notification)
        {
            var plainTextMessage = new StringBuilder();
            plainTextMessage.AppendLine($"Campaign manager invite for campaign {notification.CampaignName}");
            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine($"{notification.SenderName} has invited you have been invited you to become campaign manager for the campaign {notification.CampaignName}.");
            plainTextMessage.AppendLine("To accept go to the following URL: ");
            plainTextMessage.AppendLine(notification.AcceptUrl);
            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine("To decline go to the following Url: ");
            plainTextMessage.AppendLine(notification.DeclineUrl);

            return plainTextMessage.ToString();
        }

        private string BuildHtmlMessage(SendCampaignManagerInvite notification)
        {
            var htmlTextMessage = new StringBuilder();
            htmlTextMessage.AppendLine($"Campaign manager invite for campaign {notification.CampaignName}");
            htmlTextMessage.AppendLine();
            htmlTextMessage.AppendLine($"{notification.SenderName} has invited you have been invited you to become campaign manager for the campaign {notification.CampaignName}.");
            htmlTextMessage.AppendLine($"To accept <a href=\"{notification.AcceptUrl}\">click here</a>.");
            htmlTextMessage.AppendLine($"To decline <a href=\"{notification.DeclineUrl}\">click here</a>.");

            return htmlTextMessage.ToString();
        }
    }
}
