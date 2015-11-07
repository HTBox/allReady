using AllReady.Areas.Admin.Features.Tasks;
using Xunit;

namespace AllReady.UnitTests
{
    public class EditingTask : InMemoryContextTaskTest
    {
        public EditingTask()
        {
            // TODO: any setup of Model, etc.
        }

        [Fact]
        public void ModelIsCreated()
        {
            var sut = new EditTaskCommandHandler(Context);
            int actual = sut.Handle(new EditTaskCommand {Task = Model});
            Assert.Equal(1, actual);
        }
    }
}