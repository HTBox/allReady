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

            const int taskId = 1;
            var message = new UpdateTaskCommand { VolunteerTask = new VolunteerTask {Id = taskId} };

            using (var context = new AllReadyContext(options)) {
                context.Tasks.Add(new VolunteerTask {
                    Id = taskId,
                    RequiredSkills = new List<VolunteerTaskSkill> {
                        new VolunteerTaskSkill()
                    }
                });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new UpdateTaskCommandHandler(context);
                await sut.Handle(message);
            }

            using (var context = new AllReadyContext(options)) {
                var @task = context.Tasks.Include(t => t.RequiredSkills).FirstOrDefault(t => t.Id == taskId);
                Assert.NotNull(@task);
                Assert.Equal(@task.RequiredSkills.Count, 0);
            }
        }
    }
}
