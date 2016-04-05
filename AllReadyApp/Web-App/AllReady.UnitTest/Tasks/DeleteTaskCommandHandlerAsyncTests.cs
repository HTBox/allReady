using System.Linq;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Models;
using AllReady.UnitTest.Features.Campaigns;
using Xunit;

namespace AllReady.UnitTest.Tasks
{
    public class DeleteTaskCommandHandlerAsyncTests : InMemoryContextTest
    {
        private const int TaskId = 1;

        public DeleteTaskCommandHandlerAsyncTests()
        {
            Context.Tasks.Add(new AllReadyTask {Id=1});
            Context.SaveChanges();
        }

        [Fact]
        public void TaskIsDeleted()
        {
            var sut = new DeleteTaskCommandHandlerAsync(Context);
            sut.Handle(new DeleteTaskCommandAsync {TaskId = TaskId});
            Assert.False(Context.Tasks.Any(t=>t.Id == TaskId));
        }

        [Fact]
        public void NonExistantTaskDoesNotCauseException()
        {
            var sut = new DeleteTaskCommandHandlerAsync(Context);
            sut.Handle(new DeleteTaskCommandAsync {TaskId = 666});
            Assert.False(Context.Tasks.Any(t=>t.Id == 666));
        }
    }
}