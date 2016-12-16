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

            const int taskId = 1;
            var message = new TaskByTaskIdQuery { TaskId = taskId };

            using (var context = new AllReadyContext(options)) {
                context.Tasks.Add(new AllReadyTask {Id = taskId});
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new TaskByTaskIdQueryHandler(context);
                var @task = await sut.Handle(message);

                Assert.Equal(@task.Id, taskId);
            }
        }
    }
}