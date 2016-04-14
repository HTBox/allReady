using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Activities;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Activities
{
    public class UpdateActivityHandlerTests
    {
        [Fact]
        public async Task UpdateActivityHandlerInvokesUpdateActivityWithCorrectActivity()
        {
            var message = new UpdateActivity { Activity = new Activity() };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new UpdateActivityHandler(dataAccess.Object);
            await sut.Handle(message);

            dataAccess.Verify(x => x.UpdateActivity(message.Activity), Times.Once);
        }
    }
}
