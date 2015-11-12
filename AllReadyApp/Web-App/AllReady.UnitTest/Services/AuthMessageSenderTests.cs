
using System.Collections.Generic;
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
            bus.Verify(mock => mock.Send(It.IsAny<IRequest>()), Times.Exactly(1));
        }

        [Fact]
        public void SendSmsAsyncShouldPutCommandOnBus()
        {

            const string smsRecipient = "phoneNumber@email.com";
            const string smsMesssage = "test message body";

            var bus = MockIMediator();
            var messageSender = new AuthMessageSender(bus.Object);
            messageSender.SendSmsAsync(smsRecipient, smsMesssage);
            bus.Verify(mock => mock.Send(It.IsAny<IRequest>()), Times.Exactly(1));
        }

        /// <summary>
        /// Mocks an IMediator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private Mock<IMediator> MockIMediator()
        {
            var busMock = new Mock<IMediator>();
            busMock.Setup(mock => mock.Send(It.IsAny<IRequest>())).Verifiable();
            return busMock;
        }
    }
}
