using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Services;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Services
{
    public class AuthMessageSenderTests
    {
        [Fact]
        public async Task SendEmailAsyncShouldSendNotifyVolunteersCommand()
        {
            const string emailRecipient = "test@email.com";
            const string emailSubject = "test subject";
            const string emailMessage = "test message body";

            var mediator = MockIMediator();
            var messageSender = new AuthMessageSender(mediator.Object);
            await messageSender.SendEmailAsync(emailRecipient, emailSubject, emailMessage);

            mediator.Verify(mock => mock.SendAsync(
                It.Is<NotifyVolunteersCommand>(request => 
                request.ViewModel.EmailMessage == emailMessage
                && request.ViewModel.EmailRecipients.SequenceEqual(new List<string> {emailRecipient})
                && request.ViewModel.HtmlMessage == emailMessage
                && request.ViewModel.Subject == emailSubject)),
                Times.Exactly(1));
        }

        [Fact]
        public async Task SendSmsAsyncShouldSendNotifyVolunteersCommand()
        {
            const string smsRecipient = "phoneNumber@email.com";
            const string smsMesssage = "test message body";

            var mediator = MockIMediator();
            var messageSender = new AuthMessageSender(mediator.Object);
            await messageSender.SendSmsAsync(smsRecipient, smsMesssage);
            mediator.Verify(mock => mock.SendAsync(
                It.Is<NotifyVolunteersCommand>(request =>
                    request.ViewModel.SmsMessage == smsMesssage
                    && request.ViewModel.SmsRecipients.SequenceEqual(new List<string> { smsRecipient }))),
                Times.Exactly(1));
        }

        /// <summary>
        /// Mocks an IMediator.
        /// </summary>
        /// <returns></returns>
        private static Mock<IMediator> MockIMediator()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(mock => mock.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Returns(Task.FromResult(new Unit()))
                .Verifiable();
            return mediatorMock;
        }
    }
}