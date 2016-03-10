using AllReady.Features.Activity;
using AllReady.Models;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Activity
{
    public class DeleteActivityAndTaskSignupsCommandHandlerAsyncTests
    {
        [Fact]
        public async Task InvokesDeleteActivityAndTaskSignupsAsyncWithCorrectActivitySignupId()
        {
            var message = new DeleteActivityAndTaskSignupsCommandAsync { ActivitySignupId = 1 };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new DeleteActivityAndTaskSignupsCommandHandlerAsync(dataAccess.Object);
            await sut.Handle(message);

            dataAccess.Verify(x => x.DeleteActivityAndTaskSignupsAsync(message.ActivitySignupId));
        }
    }
}
