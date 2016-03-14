using AllReady.Features.Tasks;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Tasks
{
    public class TaskByTaskIdQueryHandlerTests
    { 
        [Fact]
        public void WhenHandlingTaskByTaskIdQueryGetTaskIsInvokedWithCorrectTaskId()
        {
            var message = new TaskByTaskIdQuery { TaskId = 1 };

            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new TaskByTaskIdQueryHandler(dataAccess.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetTask(message.TaskId), Times.Once);
        }
    }
}
