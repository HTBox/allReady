using AllReady.Features.Activity;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Activity
{
    public class ActivityByActivityIdQueryHandlerTests
    {
        [Fact]
        public void CallsGetActivityWithTheCorrectActivityId()
        {
            var message = new ActivityByActivityIdQuery { ActivityId = 1 };
            var dataAccess = new Mock<IAllReadyDataAccess>();

            var sut = new ActivityByActivityIdQueryHandler(dataAccess.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetActivity(message.ActivityId), Times.Once());
        }

        [Fact]
        public void ReturnsCorrectType()
        {
            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetActivity(It.IsAny<int>())).Returns(new Models.Activity());
            var sut = new ActivityByActivityIdQueryHandler(dataAccess.Object);
            var result = sut.Handle(new ActivityByActivityIdQuery());

            Assert.IsType<Models.Activity>(result);
        }
    }
}
