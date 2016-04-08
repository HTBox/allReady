using AllReady.Features.Activity;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Activity
{
    public class ActivitySignupByActivityIdAndUserIdQueryHandlerShould
    {
        [Fact]
        public void InvokeGetActivitySignupWithTheCorrectParameters()
        {
            var message = new ActivitySignupByActivityIdAndUserIdQuery { ActivityId = 1, UserId = "1" };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new ActivitySignupByActivityIdAndUserIdQueryHandler(dataAccess.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetActivitySignup(message.ActivityId, message.UserId), Times.Once);
        }
    }
}
