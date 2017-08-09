using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Features.Notifications;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Notifications
{
    public class SendCampaignManagerInviteHandlerShould
    {
        private const string declineUrl = "http://decline.com";
        private const string campaignName = "The campaign";
        private const string inviteeEmail = "test@test.com";
        private const string registerUrl = "http://register.com";
        private const string senderName = "John Smith";
        private const string acceptUrl = "http://accept.com";

        [Fact]
        public async Task SendInviteMessage()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new SendCampaignManagerInviteHandler(mockMediator.Object);

            await handler.Handle(new SendCampaignManagerInvite
            {
                AcceptUrl = acceptUrl,
                DeclineUrl = declineUrl,
                CampaignName = campaignName,
                InviteeEmail = inviteeEmail,
                RegisterUrl = registerUrl,
                SenderName = senderName
            });

            mockMediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(cmd =>
                cmd.ViewModel != null &&
                cmd.ViewModel.Subject == "Campaign manager invite" &&
                cmd.ViewModel.HtmlMessage == GetHtmlMessage() &&
                cmd.ViewModel.EmailMessage == GetPlainTextMessage() &&
                cmd.ViewModel.EmailRecipients.Contains(inviteeEmail)
            )), Times.Once);
        }

        private string GetHtmlMessage()
        {
            var htmlTextMessage = new StringBuilder();
            htmlTextMessage.AppendLine($"Campaign manager invite for campaign {campaignName}");
            htmlTextMessage.AppendLine();
            htmlTextMessage.AppendLine($"{senderName} has invited you have been invited you to become campaign manager for the campaign {campaignName}.");
            htmlTextMessage.AppendLine($"To accept <a href=\"{acceptUrl}\">click here</a>.");
            htmlTextMessage.AppendLine($"To decline <a href=\"{declineUrl}\">click here</a>.");

            return htmlTextMessage.ToString();
        }

        private string GetPlainTextMessage()
        {
            var plainTextMessage = new StringBuilder();
            plainTextMessage.AppendLine($"Campaign manager invite for campaign {campaignName}");
            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine($"{senderName} has invited you have been invited you to become campaign manager for the campaign {campaignName}.");
            plainTextMessage.AppendLine("To accept go to the following URL: ");
            plainTextMessage.AppendLine(acceptUrl);
            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine("To decline go to the following Url: ");
            plainTextMessage.AppendLine(declineUrl);

            return plainTextMessage.ToString();
        }
    }
}
