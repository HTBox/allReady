using AllReady.Features.Event;
using AllReady.Models;
using Moq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using MediatR;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class UnregisterEventHandlerTests
    {
        [Fact]
        public async Task InvokesDeleteEventAndTaskSignupsAsyncWithCorrectEventSignupId()
        {
            var message = new UnregisterEvent { EventSignupId = 1 };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new UnregisterEventHandler(dataAccess.Object, Mock.Of<IMediator>());
            await sut.Handle(message);

            dataAccess.Verify(x => x.DeleteEventAndTaskSignupsAsync(message.EventSignupId));
        }

        [Fact]
        public async Task PublishesUserUnenrollsWithCorrectData()
        {
            var message = new UnregisterEvent { EventSignupId = 1, UserId = "1" };
            var mediator = new Mock<IMediator>();

            var sut = new UnregisterEventHandler(Mock.Of<IAllReadyDataAccess>(), mediator.Object);
            await sut.Handle(message);

            mediator.Verify(x => x.PublishAsync(It.Is<UserUnenrolls>(y => y.EventId == message.EventSignupId && y.UserId == message.UserId)), Times.Once);
        }
    }
}
