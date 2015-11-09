using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.Models;
using Xunit;

namespace AllReady.UnitTest.Tasks
{
    public class EditingTask : InMemoryContextTest
    {
        [Fact]
        public void ModelIsCreated()
        {
            var sut = new EditTaskCommandHandler(Context);
            int actual = sut.Handle(new EditTaskCommand {Task = new TaskEditModel()});
            Assert.Equal(1, actual);
        }
    }
}