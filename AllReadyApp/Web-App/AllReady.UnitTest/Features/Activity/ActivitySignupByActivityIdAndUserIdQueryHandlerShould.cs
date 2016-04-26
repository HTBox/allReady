using AllReady.Features.Event;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class EventSignupByEventIdAndUserIdQueryHandlerShould
    {
        [Fact]
        public void InvokeGetEventSignupWithTheCorrectParameters()
        {
            var message = new EventSignupByEventIdAndUserIdQuery { EventId = 1, UserId = "1" };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new EventSignupByEventIdAndUserIdQueryHandler(dataAccess.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetEventSignup(message.EventId, message.UserId), Times.Once);
        }
    }
}
