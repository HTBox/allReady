using AllReady.Features.Notifications;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class EventManagerInviteAccepted : IAsyncNotification
    {
        public string InviteeEmail { get; set; }
        public string CampaignName { get; set; }
        public string EventName { get; set; }
        public string EventUrl { get; set; }
        public string SenderEmail { get; set; }
    }

    public class EventManagerInviteAcceptedHandler : IAsyncNotificationHandler<EventManagerInviteAccepted>
    {
        private readonly IMediator _mediator;

        public EventManagerInviteAcceptedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(EventManagerInviteAccepted notification)
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
                    Subject = "Event manager invite accepted"
                }
            };

            await _mediator.SendAsync(command);
        }

        private string BuildPlainTextMessage(EventManagerInviteAccepted notification)
        {
            return $"{notification.InviteeEmail} have accepted the invite to become event manager of the event {notification.EventName} ({notification.EventUrl}) in the campaign {notification.CampaignName}.";
        }

        private string BuildHtmlMessage(EventManagerInviteAccepted notification)
        {
            return $"{notification.InviteeEmail} have accepted the invite to become event manager of the event <a href=\"{notification.EventUrl}\">{notification.EventName}</a> in the campaign {notification.CampaignName}.";
        }
    }
}
