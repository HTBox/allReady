using AllReady.Features.Manage;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Manage
{
    public class UserByUserIdQueryHandlerShould
    {
        [Fact]
        public void InvokeGetUserWithTheCorrectUserId()
        {
            var message = new UserByUserIdQuery { UserId = "1" };
            var dataAccess = new Mock<IAllReadyDataAccess>();

            var sut = new UserByUserIdQueryHandler(dataAccess.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetUser(message.UserId), Times.Once);
        }
    }
}
