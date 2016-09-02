using AllReady.Features.Event;
using AllReady.Models;
using System.Linq;
using Moq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using MediatR;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class UnregisterEventHandlerTests : InMemoryContextTest
    {
        [Fact]
        public async Task InvokesDeleteEventAndTaskSignupsAsyncWithCorrectEventSignupId()
        {
            const int eventSignupId = 1;
            var message = new UnregisterEvent { EventSignupId = eventSignupId };
            var sut = new UnregisterEventHandler(this.Context, Mock.Of<IMediator>());
            await sut.Handle(message);

            var result = Context.EventSignup.SingleOrDefault(x => x.Id == eventSignupId);

            Assert.Equal(result, null);
        }

        [Fact]
        public async Task PublishesUserUnenrollsWithCorrectData()
        {
            var message = new UnregisterEvent { EventSignupId = 1, UserId = "1" };
            var mediator = new Mock<IMediator>();

            var sut = new UnregisterEventHandler(this.Context, mediator.Object);
            await sut.Handle(message);

            mediator.Verify(x => x.PublishAsync(It.Is<UserUnenrolls>(y => y.EventId == message.EventSignupId && y.UserId == message.UserId)), Times.Once);
        }
    }
}
