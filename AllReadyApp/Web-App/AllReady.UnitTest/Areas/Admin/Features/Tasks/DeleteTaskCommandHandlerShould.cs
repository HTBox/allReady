using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class DeleteTaskCommandHandlerShould : InMemoryContextTest
    {
        private const int TaskId = 1;

        public DeleteTaskCommandHandlerShould()
        {
            Context.Tasks.Add(new VolunteerTask { Id = 1 });
            Context.SaveChanges();
        }

        [Fact]
        public async Task DeleteTask()
        {
            var sut = new DeleteTaskCommandHandler(Context);
            await sut.Handle(new DeleteTaskCommand { TaskId = TaskId });
            Assert.False(Context.Tasks.Any(t => t.Id == TaskId));
        }
    }
}