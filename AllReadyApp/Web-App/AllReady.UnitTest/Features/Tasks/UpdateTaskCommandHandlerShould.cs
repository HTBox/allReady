using System.Threading.Tasks;
using AllReady.Features.Tasks;
using AllReady.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Xunit;

namespace AllReady.UnitTest.Features.Tasks
{
    public class UpdateTaskCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task HandleInvokesUpdateTaskAsyncWithCorrectData()
        {
            var options = this.CreateNewContextOptions();

            const int volunteerTaskId = 1;
            var message = new UpdateVolunteerTaskCommand { VolunteerTask = new VolunteerTask {Id = volunteerTaskId} };

            using (var context = new AllReadyContext(options)) {
                context.VolunteerTasks.Add(new VolunteerTask {
                    Id = volunteerTaskId,
                    RequiredSkills = new List<VolunteerTaskSkill> {
                        new VolunteerTaskSkill()
                    }
                });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new UpdateVolunteerTaskCommandHandler(context);
                await sut.Handle(message);
            }

            using (var context = new AllReadyContext(options)) {
                var volunteerTask = context.VolunteerTasks.Include(t => t.RequiredSkills).FirstOrDefault(t => t.Id == volunteerTaskId);
                Assert.NotNull(volunteerTask);
                Assert.Empty(volunteerTask.RequiredSkills);
            }
        }
    }
}
