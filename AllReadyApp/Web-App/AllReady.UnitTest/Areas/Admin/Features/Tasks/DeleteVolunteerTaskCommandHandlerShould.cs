using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class DeleteVolunteerTaskCommandHandlerShould : InMemoryContextTest
    {
        private const int VolunteerTaskId = 1;

        public DeleteVolunteerTaskCommandHandlerShould()
        {
            Context.VolunteerTasks.Add(new VolunteerTask { Id = 1 });
            Context.SaveChanges();
        }

        [Fact]
        public async Task DeleteVolunteerTask()
        {
            var sut = new DeleteVolunteerTaskCommandHandler(Context);
            await sut.Handle(new DeleteVolunteerTaskCommand { VolunteerTaskId = VolunteerTaskId });
            Assert.False(Context.VolunteerTasks.Any(t => t.Id == VolunteerTaskId));
        }
    }
}