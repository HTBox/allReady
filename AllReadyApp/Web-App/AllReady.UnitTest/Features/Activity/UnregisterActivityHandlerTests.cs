using AllReady.Features.Activity;
using AllReady.Models;
using Moq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using MediatR;
using Xunit;

namespace AllReady.UnitTest.Features.Activity
{
    public class UnregisterActivityHandlerTests
    {
        [Fact]
        public async Task InvokesDeleteActivityAndTaskSignupsAsyncWithCorrectActivitySignupId()
        {
            var message = new UnregisterActivity { ActivitySignupId = 1 };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new UnregisterActivityHandler(dataAccess.Object, Mock.Of<IMediator>());
            await sut.Handle(message);

            dataAccess.Verify(x => x.DeleteActivityAndTaskSignupsAsync(message.ActivitySignupId));
        }

        [Fact]
        public async Task PublishesUserUnenrollsWithCorrectData()
        {
            var message = new UnregisterActivity { ActivitySignupId = 1, UserId = "1" };
            var mediator = new Mock<IMediator>();

            var sut = new UnregisterActivityHandler(Mock.Of<IAllReadyDataAccess>(), mediator.Object);
            await sut.Handle(message);

            mediator.Verify(x => x.PublishAsync(It.Is<UserUnenrolls>(y => y.ActivityId == message.ActivitySignupId && y.UserId == message.UserId)), Times.Once);
        }
    }
}
