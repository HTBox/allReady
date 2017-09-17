using System.Threading.Tasks;
using AllReady.Features.Tasks;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Features.Tasks
{
    using System.Linq;

    public class AddTaskCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task HandleInvokesAddTaskAsyncWithCorrectData()
        {
            var options = this.CreateNewContextOptions();

            using (var context = new AllReadyContext(options)) {
                var sut = new AddVolunteerTaskCommandHandler(context);
                var message = new AddVolunteerTaskCommand { VolunteerTask = new VolunteerTask() };
                await sut.Handle(message);
            }

            using (var context = new AllReadyContext(options)) {
                var volunteerTasks = context.VolunteerTasks.Count();
                Assert.Equal(1, volunteerTasks);
            }
        }
    }
}
