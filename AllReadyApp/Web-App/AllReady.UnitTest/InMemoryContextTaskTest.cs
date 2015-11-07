using AllReady.Areas.Admin.ViewModels;

namespace AllReady.UnitTests
{
    public abstract class InMemoryContextTaskTest : InMemoryContextTest
    {
        protected TaskEditViewModel Model = new TaskEditViewModel();

        protected InMemoryContextTaskTest()
        {
            // TODO: setup Model in any way for any other tests
        }
    }
}