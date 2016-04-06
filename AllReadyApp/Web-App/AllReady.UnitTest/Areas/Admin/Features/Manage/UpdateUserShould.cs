using System.Threading.Tasks;
using AllReady.Features.Manage;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Manage
{
    public class UpdateUserShould
    {
        [Fact]
        public async Task InvokeUpdateUserWithTheCorrectUser()
        {
            var message = new UpdateUser { User = new ApplicationUser() };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new UpdateUserHandler(dataAccess.Object);
            await sut.Handle(message);

            dataAccess.Verify(x => x.UpdateUser(message.User), Times.Once);
        }
    }
}
