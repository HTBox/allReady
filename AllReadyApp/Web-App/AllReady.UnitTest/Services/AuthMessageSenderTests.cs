
using System.Collections.Generic;
using System.Linq;
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
        public void SendEmailAsyncShouldPutCommandOnBus()
        {
            const string emailRecipient = "test@email.com";
            const string emailSubject = "test subject";
            const string emailMessage = "test message body";

            var bus = MockIMediator();
            var messageSender = new AuthMessageSender(bus.Object);
            messageSender.SendEmailAsync(emailRecipient, emailSubject, emailMessage);

            bus.Verify(mock => mock.Send(
                It.Is<NotifyVolunteersCommand>(request => 
                request.ViewModel.EmailMessage == emailMessage
                && request.ViewModel.EmailRecipients.SequenceEqual(new List<string> {emailRecipient})
                && request.ViewModel.HtmlMessage == emailMessage
                && request.ViewModel.Subject == emailSubject)),
                Times.Exactly(1));
        }

        [Fact]
        public void SendSmsAsyncShouldPutCommandOnBus()
        {

            const string smsRecipient = "phoneNumber@email.com";
            const string smsMesssage = "test message body";

            var bus = MockIMediator();
            var messageSender = new AuthMessageSender(bus.Object);
            messageSender.SendSmsAsync(smsRecipient, smsMesssage);
            bus.Verify(mock => mock.Send(
                It.Is<NotifyVolunteersCommand>(request => 
                request.ViewModel.SmsMessage == smsMesssage
                && request.ViewModel.SmsRecipients.SequenceEqual(new List<string> {smsRecipient}))),
                Times.Exactly(1));
        }

        /// <summary>
        /// Mocks an IMediator.
        /// </summary>
        /// <returns></returns>
        private static Mock<IMediator> MockIMediator()
        {
            var busMock = new Mock<IMediator>();
            busMock.Setup(mock => mock.Send(It.IsAny<NotifyVolunteersCommand>())).Verifiable();
            return busMock;
        }
    }
}
