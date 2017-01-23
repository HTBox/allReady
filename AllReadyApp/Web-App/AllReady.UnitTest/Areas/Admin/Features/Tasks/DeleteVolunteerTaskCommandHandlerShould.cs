using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class DeleteVolunteerTaskCommandHandlerShould : InMemoryContextTest
    {
        private const int TaskId = 1;

        public DeleteVolunteerTaskCommandHandlerShould()
        {
            Context.Tasks.Add(new VolunteerTask { Id = 1 });
            Context.SaveChanges();
        }

        [Fact]
        public async Task DeleteVolunteerTask()
        {
            var sut = new DeleteVolunteerTaskCommandHandler(Context);
            await sut.Handle(new DeleteVolunteerTaskCommand { TaskId = TaskId });
            Assert.False(Context.Tasks.Any(t => t.Id == TaskId));
        }
    }
}