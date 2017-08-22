using AllReady.Features.Tasks;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Tasks
{
    using System.Threading.Tasks;

    public class TaskByTaskIdQueryHandlerShould : InMemoryContextTest
    {
        private const int VolunteerTaskId = 1;

        protected override void LoadTestData()
        {
            Context.VolunteerTasks.Add(new VolunteerTask { Id = VolunteerTaskId, Organization = new Organization(), Event = new Event{ Campaign = new Campaign()}});
            Context.SaveChanges();
        }

        [Fact]
        public async Task WhenHandlingTaskByTaskIdQueryGetTaskIsInvokedWithCorrectTaskId()
        {
            var message = new VolunteerTaskByVolunteerTaskIdQuery { VolunteerTaskId = VolunteerTaskId };
            
            var sut = new VolunteerTaskByVolunteerTaskIdQueryHandler(Context);
            var volunteerTask = await sut.Handle(message);

            Assert.Equal(volunteerTask.Id, VolunteerTaskId);
        }
    }
}
