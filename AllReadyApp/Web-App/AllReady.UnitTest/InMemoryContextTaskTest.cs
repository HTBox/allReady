using AllReady.Areas.Admin.ViewModels;
using AllReady.Models;
using AllReady.UnitTest;

namespace AllReady.UnitTests
{
    public abstract class InMemoryContextTest : TestBase
    {
        protected AllReadyContext Context;

        protected InMemoryContextTest()
        {
            Context = (AllReadyContext) ServiceProvider.GetService(typeof(AllReadyContext));

            // TODO: setup Model in any way for any other tests
        }
    }
}