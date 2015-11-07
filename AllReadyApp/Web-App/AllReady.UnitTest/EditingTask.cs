using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels;
using Xunit;

namespace AllReady.UnitTests
{
    public class EditingTask : InMemoryContextTest
    {
        [Fact]
        public void ModelIsCreated()
        {
            var sut = new EditTaskCommandHandler(Context);
            int actual = sut.Handle(new EditTaskCommand {Task = new TaskEditViewModel()});
            Assert.Equal(1, actual);
        }
    }
}