using AllReady.Features.Tasks;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Tasks
{
    using System.Threading.Tasks;

    public class TaskByTaskIdQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task WhenHandlingTaskByTaskIdQueryGetTaskIsInvokedWithCorrectTaskId() {
            var options = this.CreateNewContextOptions();

            const int volunteerTaskId = 1;
            var message = new VolunteerTaskByVolunteerTaskIdQuery { VolunteerTaskId = volunteerTaskId };

            using (var context = new AllReadyContext(options)) {
                context.VolunteerTasks.Add(new VolunteerTask {Id = volunteerTaskId});
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new VolunteerTaskByVolunteerTaskIdQueryHandler(context);
                var volunteerTask = await sut.Handle(message);

                Assert.Equal(volunteerTask.Id, volunteerTaskId);
            }
        }
    }
}