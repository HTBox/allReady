using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Features.Notifications;
using MediatR;
using Moq;
using System.Threading.Tasks;

namespace AllReady.UnitTest.Areas.Admin.Features.Notifications
{
    public class CampaignManagerInviteAcceptedHandlerShould
    {
        private const string campaignName = "My campaign";
        private const string campaignUrl = "http://mycampaign.com/";
        private const string inviteeEmail = "invitee@test.com";
        private const string senderEmail = "sender@test.com";

        public async Task SendConfirmationEmailToInviter()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var handler = new CampaignManagerInviteAcceptedHandler(mockMediator.Object);

            var notification = new CampaignManagerInviteAccepted()
            {
                CampaignName = campaignName,
                CampaignUrl = campaignUrl,
                InviteeEmail = inviteeEmail,
                SenderEmail = senderEmail
            };

            // Act
            await handler.Handle(notification);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(cmd =>
                cmd.ViewModel != null &&
                cmd.ViewModel.Subject == "Campaign manager invite accepted" &&
                cmd.ViewModel.HtmlMessage == $"{notification.InviteeEmail}  have accepted the invite to become campaign manager of the campaign <a href=\"{notification.CampaignUrl}\">{notification.CampaignName}</a>" &&
                cmd.ViewModel.EmailMessage == $"{notification.InviteeEmail} have accepted the invite to become campaign manager of the campaign {notification.CampaignName} ({notification.CampaignUrl})." &&
                cmd.ViewModel.EmailRecipients.Contains(inviteeEmail)
            )), Times.Once);
        }
    }
}
