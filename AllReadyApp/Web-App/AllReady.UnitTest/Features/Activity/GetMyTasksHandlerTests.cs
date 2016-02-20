using System.Linq;
using AllReady.Features.Activity;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Activity
{
    public class GetMyTasksHandlerTests
    {
        [Fact]
        public void ReturnsExpectedTasks()
        {
            var mockDbAccess = new Mock<IAllReadyDataAccess>();

            var command = new GetMyTasksCommand {ActivityId = 1, UserId = "9D0929AC-BE6A-4A0B-A758-6C6FC31A8C47"};
            mockDbAccess.Setup(db => db.GetTasksAssignedToUser(command.ActivityId, command.UserId))
                        .Returns(new[] {new TaskSignup {Id = 1}});

            var sut = new GetMyTasksHandler(mockDbAccess.Object);
            var response = sut.Handle(command);

            Assert.True(response.Any());
        }
    }
}
