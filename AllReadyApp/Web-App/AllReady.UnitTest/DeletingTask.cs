using System.Linq;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTests
{
    public class DeletingTask : InMemoryContextTaskTest
    {
        const int TaskId = 1;

        public DeletingTask()
        {
            Context.Tasks.Add(new AllReadyTask {Id=1});
        }

        [Fact]
        public void TaskIsDeleted()
        {
            var sut = new DeleteTaskCommandHandler(Context);
            sut.Handle(new DeleteTaskCommand {TaskId = TaskId});
            Assert.False(Context.Tasks.Any(t=>t.Id == TaskId));
        }

        [Fact]
        public void NonExistantTaskDoesNotCauseException()
        {
            var sut = new DeleteTaskCommandHandler(Context);
            sut.Handle(new DeleteTaskCommand {TaskId = 666});
            Assert.False(Context.Tasks.Any(t=>t.Id == TaskId));
        }
    }
}