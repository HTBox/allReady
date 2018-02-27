using AllReady.Features.Notifications;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class CampaignManagerInviteAccepted : IAsyncNotification
    {
        public string InviteeEmail { get; set; }
        public string CampaignName { get; set; }
        public string CampaignUrl { get; set; }
        public string SenderEmail { get; set; }
    }

    public class CampaignManagerInviteAcceptedHandler : IAsyncNotificationHandler<CampaignManagerInviteAccepted>
    {
        private readonly IMediator _mediator;

        public CampaignManagerInviteAcceptedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(CampaignManagerInviteAccepted notification)
        {
            var plainTextMessage = BuildPlainTextMessage(notification);
            var htmlMessage = BuildHtmlMessage(notification);

            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    EmailMessage = plainTextMessage.ToString(),
                    EmailRecipients = new List<string> { notification.SenderEmail },
                    HtmlMessage = htmlMessage,
                    Subject = "Campaign manager invite accepted"
                }
            };

            await _mediator.SendAsync(command);
        }

        private string BuildPlainTextMessage(CampaignManagerInviteAccepted notification)
        {
            return $"{notification.InviteeEmail} have accepted the invite to become campaign manager of the campaign {notification.CampaignName} ({notification.CampaignUrl}).";
        }

        private string BuildHtmlMessage(CampaignManagerInviteAccepted notification)
        {
            return $"{notification.InviteeEmail} have accepted the invite to become campaign manager of the campaign <a href=\"{notification.CampaignUrl}\">{notification.CampaignName}</a>";
        }
    }
}
