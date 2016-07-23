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
        private AllReadyTask _task;
        public TaskListQueryHandlerTests()
        {
            handler = new TaskListQueryHandler(Context);
        }

        private AllReadyTask GenerateTask()
        {
            return new AllReadyTask
            {
                Event = new Event(),
                Name = "TestTask",
                StartDateTime = new DateTimeOffset(DateTime.Today),
                EndDateTime = new DateTimeOffset(DateTime.Today.AddDays(1)),
                NumberOfVolunteersRequired = 2
            };
        }

        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            _task = GenerateTask();
            context.Tasks.Add(_task);
            context.SaveChanges();
        }

        [Fact]
        public void TaskListQueryMade()
        {
            var taskListQuery = new TaskListQuery
            {
                EventId = _task.Id
            };
            var result = handler.Handle(taskListQuery).ToList();
            Assert.NotNull(result);

            Assert.Equal(result.Count, 1);

            var task = result.First();
            Assert.Equal(task.Id, _task.Id);
            Assert.Equal(task.Name, _task.Name);
            Assert.Equal(task.StartDateTime, _task.StartDateTime);
            Assert.Equal(task.EndDateTime, _task.EndDateTime);
            Assert.Equal(task.NumberOfVolunteersRequired, _task.NumberOfVolunteersRequired);
            Assert.False(task.IsUserSignedUpForTask);
        }
    }
}
