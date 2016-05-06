using AllReady.Areas.Admin.Features.Users;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Users
{
    public class AllUsersQueryHandlerShould
    {
        [Fact]
        public void InvokeUsers()
        {
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new AllUsersQueryHandler(dataAccess.Object);
            sut.Handle(new AllUsersQuery());

            dataAccess.Verify(x => x.Users, Times.Once);
        }
    }
}
