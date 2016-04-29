using System.Linq;
using AllReady.Features.Event;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class GetMyTasksQueryHandlerTests
    {
        [Fact]
        public void ReturnsExpectedTasks()
        {
            var mockDbAccess = new Mock<IAllReadyDataAccess>();

            var command = new GetMyTasksQuery {EventId = 1, UserId = "9D0929AC-BE6A-4A0B-A758-6C6FC31A8C47"};
            mockDbAccess.Setup(db => db.GetTasksAssignedToUser(command.EventId, command.UserId))
                        .Returns(new[] {new TaskSignup {Id = 1}});

            var sut = new GetMyTasksQueryHandler(mockDbAccess.Object);
            var response = sut.Handle(command);

            Assert.True(response.Any());
        }
    }
}
