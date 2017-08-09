using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Features.Notifications;
using MediatR;
using Moq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Notifications
{
    public class SendEventManagerInviteHandlerShould
    {
        private const string declineUrl = "http://decline.com";
        private const string eventName = "The event";
        private const string inviteeEmail = "test@test.com";
        private const string registerUrl = "http://register.com";
        private const string senderName = "John Smith";
        private const string acceptUrl = "http://accept.com";

        [Fact]
        public async Task SendInviteMessage()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new SendEventManagerInviteHandler(mockMediator.Object);

            await handler.Handle(new SendEventManagerInvite
            {
                AcceptUrl = acceptUrl,
                DeclineUrl = declineUrl,
                EventName = eventName,
                InviteeEmail = inviteeEmail,
                RegisterUrl = registerUrl,
                SenderName = senderName
            });

            mockMediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(cmd =>
                cmd.ViewModel != null &&
                cmd.ViewModel.Subject == "Event manager invite" &&
                cmd.ViewModel.HtmlMessage == GetHtmlMessage() &&
                cmd.ViewModel.EmailMessage == GetPlainTextMessage() &&
                cmd.ViewModel.EmailRecipients.Contains(inviteeEmail)
            )), Times.Once);
        }

        private string GetHtmlMessage()
        {
            var htmlTextMessage = new StringBuilder();
            htmlTextMessage.AppendLine($"Event manager invite for event {eventName}");
            htmlTextMessage.AppendLine();
            htmlTextMessage.AppendLine($"{senderName} has invited you have been invited you to become event manager for the event {eventName}.");
            htmlTextMessage.AppendLine($"To accept <a href=\"{acceptUrl}\">click here</a>.");
            htmlTextMessage.AppendLine($"To decline <a href=\"{declineUrl}\">click here</a>.");

            return htmlTextMessage.ToString();
        }

        private string GetPlainTextMessage()
        {
            var plainTextMessage = new StringBuilder();
            plainTextMessage.AppendLine($"Event manager invite for event {eventName}");
            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine($"{senderName} has invited you have been invited you to become event manager for the event {eventName}.");
            plainTextMessage.AppendLine("To accept go to the following URL: ");
            plainTextMessage.AppendLine(acceptUrl);
            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine("To decline go to the following Url: ");
            plainTextMessage.AppendLine(declineUrl);

            return plainTextMessage.ToString();
        }
    }
}
