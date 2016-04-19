using System.Threading.Tasks;
using AllReady.Features.Tasks;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Tasks
{
    public class UpdateTaskCommandHandlerAsyncTests
    {
        [Fact]
        public async Task HandleInvokesUpdateTaskAsyncWithCorrectData()
        {
            var message = new UpdateTaskCommandAsync { AllReadyTask = new AllReadyTask() };
            var dataAccess = new Mock<IAllReadyDataAccess>();

            var sut = new UpdateTaskCommandHandlerAsync(dataAccess.Object);
            await sut.Handle(message);

            dataAccess.Verify(x => x.UpdateTaskAsync(message.AllReadyTask), Times.Once);
        }
    }
}
