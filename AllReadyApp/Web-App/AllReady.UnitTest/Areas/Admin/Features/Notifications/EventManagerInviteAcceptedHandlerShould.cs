using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Features.Notifications;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AllReady.UnitTest.Areas.Admin.Features.Notifications
{
    public class EventManagerInviteAcceptedHandlerShould
    {
        private const string campaignName = "My campaign";
        private const string eventName = "My Event";
        private const string eventUrl = "http://myevent.com/";
        private const string inviteeEmail = "invitee@test.com";
        private const string senderEmail = "sender@test.com";

        public async Task SendConfirmationEmailToInviter()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var handler = new EventManagerInviteAcceptedHandler(mockMediator.Object);

            var notification = new EventManagerInviteAccepted()
            {
                CampaignName = campaignName,
                EventName = eventName,
                EventUrl = eventUrl,
                InviteeEmail = inviteeEmail,
                SenderEmail = senderEmail
            };

            // Act
            await handler.Handle(notification);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(cmd =>
                cmd.ViewModel != null &&
                cmd.ViewModel.Subject == "Event manager invite accepted" &&
                cmd.ViewModel.HtmlMessage == $"{notification.InviteeEmail}  have accepted the invite to become event manager of the event <a href=\"{notification.EventUrl}\">{notification.EventName}</a> in the campaign {notification.CampaignName}." &&
                cmd.ViewModel.EmailMessage == $"{notification.InviteeEmail} have accepted the invite to become event manager of the event {notification.EventName} ({notification.EventUrl}) in the campaign {notification.CampaignName}." &&
                cmd.ViewModel.EmailRecipients.Contains(inviteeEmail)
            )), Times.Once);
        }
    }
}
