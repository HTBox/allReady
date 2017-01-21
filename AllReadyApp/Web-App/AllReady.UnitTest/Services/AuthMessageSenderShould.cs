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
    public class AuthMessageSenderShould
    {
        [Fact]
        public async Task SendNotifyVolunteersCommandWhenInvokingSendEmailAsync()
        {
            const string emailRecipient = "test@email.com";
            const string emailSubject = "test subject";
            const string emailMessage = "test message body";

            var mediator = MockIMediator();
            var messageSender = new AuthMessageSender(mediator.Object, Mock.Of<IQueueStorageService>());
            await messageSender.SendEmailAsync(emailRecipient, emailSubject, emailMessage);

            mediator.Verify(mock => mock.SendAsync(
                It.Is<NotifyVolunteersCommand>(request =>
                request.ViewModel.EmailMessage == emailMessage
                && request.ViewModel.EmailRecipients.SequenceEqual(new List<string> { emailRecipient })
                && request.ViewModel.HtmlMessage == emailMessage
                && request.ViewModel.Subject == emailSubject)),
                Times.Exactly(1));
        }

        [Fact]
        public async Task InvokeSendSmsAsyncWithTheCorrectQueueNameAndMessage()
        {
            const string phoneNumber = "phoneNumber@email.com";
            const string smsMesssage = "test message body";

            var mediator = MockIMediator();
            var queueStorageService = new Mock<IQueueStorageService>();

            var messageSender = new AuthMessageSender(mediator.Object, queueStorageService.Object);
            await messageSender.SendSmsAsync(phoneNumber, smsMesssage);

            queueStorageService.Verify(x => x.SendMessageAsync(QueueStorageService.Queues.SmsQueue, 
                @"{""Recipient"":""phoneNumber@email.com"",""Message"":""test message body""}"));
        }

        private static Mock<IMediator> MockIMediator()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(mock => mock.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .ReturnsAsync(new Unit())
                .Verifiable();
            return mediatorMock;
        }
    }
}