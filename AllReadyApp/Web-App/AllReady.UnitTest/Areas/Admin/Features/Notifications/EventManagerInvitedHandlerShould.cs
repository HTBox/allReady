using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Features.Notifications;
using MediatR;
using Moq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Notifications
{
    public class EventManagerInvitedHandlerShould
    {
        private const string declineUrl = "http://decline.com";
        private const string eventName = "The event";
        private const string inviteeEmail = "test@test.com";
        private const string registerUrl = "http://register.com";
        private const string senderName = "John Smith";
        private const string acceptUrl = "http://accept.com";
        private const string message = "test message";

        [Fact]
        public async Task SendInviteMessageForRegisteredUser()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new EventManagerInvitedHandler(mockMediator.Object);

            await handler.Handle(new EventManagerInvited
            {
                AcceptUrl = acceptUrl,
                DeclineUrl = declineUrl,
                EventName = eventName,
                InviteeEmail = inviteeEmail,
                RegisterUrl = registerUrl,
                SenderName = senderName,
                IsInviteeRegistered = true,
                Message = message
            });

            mockMediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(cmd =>
                cmd.ViewModel != null &&
                cmd.ViewModel.Subject == "Event manager invite" &&
                cmd.ViewModel.HtmlMessage == GetHtmlMessageRegisteredUser() &&
                cmd.ViewModel.EmailMessage == GetPlainTextMessageRegisteredUser() &&
                cmd.ViewModel.EmailRecipients.Contains(inviteeEmail)
            )), Times.Once);
        }

        [Fact]
        public async Task SendInviteMessageForUnregisteredUser()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new EventManagerInvitedHandler(mockMediator.Object);

            await handler.Handle(new EventManagerInvited
            {
                AcceptUrl = acceptUrl,
                DeclineUrl = declineUrl,
                EventName = eventName,
                InviteeEmail = inviteeEmail,
                RegisterUrl = registerUrl,
                SenderName = senderName,
                IsInviteeRegistered = false,
                Message = message
            });

            mockMediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(cmd =>
                cmd.ViewModel != null &&
                cmd.ViewModel.Subject == "Event manager invite" &&
                cmd.ViewModel.HtmlMessage == GetHtmlMessageUnregisteredUser() &&
                cmd.ViewModel.EmailMessage == GetPlainTextMessageUnregisteredUser() &&
                cmd.ViewModel.EmailRecipients.Contains(inviteeEmail)
            )), Times.Once);
        }

        private string GetPlainTextMessageUnregisteredUser()
        {
            var plainTextMessage = new StringBuilder();
            plainTextMessage.AppendLine($"Event manager invite for event {eventName}");
            plainTextMessage.AppendLine();
            plainTextMessage.Append($"{senderName} has invited you to become event manager for the event {eventName}");

            plainTextMessage.Append(" with the following message:");
            plainTextMessage.AppendLine();
            plainTextMessage.Append(message);
            plainTextMessage.AppendLine();

            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine("To accept go to the following URL: ");
            plainTextMessage.AppendLine(acceptUrl);
            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine("To decline go to the following Url: ");
            plainTextMessage.AppendLine(declineUrl);

            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine("Before accepting the invite you need to register with Allready by clicking at this link: ");
            plainTextMessage.AppendLine(registerUrl);

            return plainTextMessage.ToString();
        }

        private string GetHtmlMessageUnregisteredUser()
        {
            var htmlTextMessage = new StringBuilder();
            htmlTextMessage.AppendLine($"Event manager invite for event {eventName}");
            htmlTextMessage.AppendLine();
            htmlTextMessage.Append($"{senderName} has invited you to become event manager for the event {eventName}");

            htmlTextMessage.Append(" with the following message:");
            htmlTextMessage.AppendLine();
            htmlTextMessage.AppendLine();
            htmlTextMessage.Append(message);
            htmlTextMessage.AppendLine();
            htmlTextMessage.AppendLine();

            htmlTextMessage.AppendLine($"To accept <a href=\"{acceptUrl}\">click here</a>.");
            htmlTextMessage.AppendLine($"To decline <a href=\"{declineUrl}\">click here</a>.");

            htmlTextMessage.AppendLine();
            htmlTextMessage.AppendLine($"Before accepting the invite you need to <a href=\"{registerUrl}\">register with Allready</a>.");

            return htmlTextMessage.ToString();
        }

        private string GetHtmlMessageRegisteredUser()
        {
            var htmlTextMessage = new StringBuilder();
            htmlTextMessage.AppendLine($"Event manager invite for event {eventName}");
            htmlTextMessage.AppendLine();
            htmlTextMessage.Append($"{senderName} has invited you to become event manager for the event {eventName}");

            htmlTextMessage.Append(" with the following message:");
            htmlTextMessage.AppendLine();
            htmlTextMessage.AppendLine();
            htmlTextMessage.Append(message);
            htmlTextMessage.AppendLine();
            htmlTextMessage.AppendLine();

            htmlTextMessage.AppendLine($"To accept <a href=\"{acceptUrl}\">click here</a>.");
            htmlTextMessage.AppendLine($"To decline <a href=\"{declineUrl}\">click here</a>.");

            return htmlTextMessage.ToString();
        }

        private string GetPlainTextMessageRegisteredUser()
        {
            var plainTextMessage = new StringBuilder();
            plainTextMessage.AppendLine($"Event manager invite for event {eventName}");
            plainTextMessage.AppendLine();
            plainTextMessage.Append($"{senderName} has invited you to become event manager for the event {eventName}");

            plainTextMessage.Append(" with the following message:");
            plainTextMessage.AppendLine();
            plainTextMessage.Append(message);
            plainTextMessage.AppendLine();

            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine("To accept go to the following URL: ");
            plainTextMessage.AppendLine(acceptUrl);
            plainTextMessage.AppendLine();
            plainTextMessage.AppendLine("To decline go to the following Url: ");
            plainTextMessage.AppendLine(declineUrl);

            return plainTextMessage.ToString();
        }


    }
}
