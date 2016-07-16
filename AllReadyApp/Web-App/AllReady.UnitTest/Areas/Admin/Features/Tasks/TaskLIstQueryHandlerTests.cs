using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class TaskListQueryHandlerTests : InMemoryContextTest
    {
        private readonly TaskListQueryHandler handler;
        public TaskListQueryHandlerTests()
        {
            handler = new TaskListQueryHandler(Context);
        }

        private AllReadyTask GenerateTask()
        {
            return new AllReadyTask
            {
                Id = 1,
                Event = new Event
                {
                    Id = 1
                },
                Name = "TestTask",
                StartDateTime = new DateTimeOffset(DateTime.Today),
                EndDateTime = new DateTimeOffset(DateTime.Today.AddDays(1)),
                NumberOfVolunteersRequired = 2
            };
        }

        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();

            var task = GenerateTask();
            context.Tasks.Add(task);
            context.SaveChanges();
        }

        [Fact]
        public void TaskListQueryMade()
        {
            var taskListQuery = new TaskListQuery
            {
                EventId = 1
            };
            var result = handler.Handle(taskListQuery).ToList();
            Assert.NotNull(result);

            Assert.Equal(result.Count, 1);

            var task = result.First();
            var expectedTask = GenerateTask();
            Assert.Equal(task.Id, expectedTask.Id);
            Assert.Equal(task.Name, expectedTask.Name);
            Assert.Equal(task.StartDateTime, expectedTask.StartDateTime);
            Assert.Equal(task.EndDateTime, expectedTask.EndDateTime);
            Assert.Equal(task.NumberOfVolunteersRequired, expectedTask.NumberOfVolunteersRequired);
            Assert.False(task.IsUserSignedUpForTask);
        }
    }
}
