using AllReady.Features.Event;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class EventByEventIdQueryHandlerTests
    {
        [Fact]
        public void CallsGetEventWithTheCorrectEventId()
        {
            var message = new EventByIdQuery { EventId = 1 };
            var dataAccess = new Mock<IAllReadyDataAccess>();

            var sut = new EventByIdQueryHandler(dataAccess.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetEvent(message.EventId), Times.Once());
        }

        [Fact]
        public void ReturnsCorrectType()
        {
            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetEvent(It.IsAny<int>())).Returns(new Models.Event());
            var sut = new EventByIdQueryHandler(dataAccess.Object);
            var result = sut.Handle(new EventByIdQuery());

            Assert.IsType<Models.Event>(result);
        }
    }
}
