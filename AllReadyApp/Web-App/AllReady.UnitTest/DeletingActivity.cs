using AllReady.Areas.Admin.Features.Activities;
using Xunit;
using System.Linq;

namespace AllReady.UnitTests
{
    public class DeletingActivity : InMemoryContextTest
    {
        private const int ActivityId = 1;

        [Fact]
        public void ActivityIsDeleted()
        {
            var sut = new DeleteActivityCommandHandler(Context);
            sut.Handle(new DeleteActivityCommand { ActivityId = ActivityId });
            Assert.False(Context.Activities.Any(t => t.Id == ActivityId));
        }

        [Fact]
        public void NonExistantTaskDoesNotCauseException()
        {
            var sut = new DeleteActivityCommandHandler(Context);
            sut.Handle(new DeleteActivityCommand { ActivityId = 666 });
            Assert.False(Context.Activities.Any(t => t.Id == ActivityId));
        }
    }
}