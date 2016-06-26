using System.Threading.Tasks;
using AllReady.Features.Tasks;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Tasks
{
    public class AddTaskCommandHandlerAsyncTests
    {
        [Fact]
        public async Task HandleInvokesAddTaskAsyncWithCorrectData()
        {
            var message = new AddTaskCommandAsync { AllReadyTask = new AllReadyTask() };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new AddTaskCommandHandlerAsync(dataAccess.Object);
            await sut.Handle(message);

            dataAccess.Verify(x => x.AddTaskAsync(message.AllReadyTask), Times.Once);
        }
    }
}
