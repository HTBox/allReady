using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using System.Text;
using AllReady.Features.Notifications;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class CampaignManagerInvitedHandler : IAsyncNotificationHandler<CampaignManagerInvited>
    {
        private IMediator _mediator;

        public CampaignManagerInvitedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(CampaignManagerInvited notification)
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

        private string BuildPlainTextMessage(CampaignManagerInvited notification)
        {
            var plainTextMessage = new StringBuilder();
            plainTextMessage.AppendLine($"Campaign manager invite for campaign {notification.CampaignName}");
            plainTextMessage.AppendLine();
            plainTextMessage.Append($"{notification.SenderName} has invited you to become campaign manager for the campaign {notification.CampaignName}");

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

        private string BuildHtmlMessage(CampaignManagerInvited notification)
        {
            var htmlTextMessage = new StringBuilder();
            htmlTextMessage.AppendLine($"Campaign manager invite for campaign {notification.CampaignName}");
            htmlTextMessage.AppendLine();
            htmlTextMessage.Append($"{notification.SenderName} has invited you to become campaign manager for the campaign {notification.CampaignName}");

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
            htmlTextMessage.AppendLine();
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
