using AllReady.Models;
using AllReady.UnitTest;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace AllReady.UnitTest
{
    public abstract class InMemoryContextTestBase : TestBase
    {
        /// <summary>
        /// Gets the in-memory database context.
        /// </summary>
        protected AllReadyContext Context { get; private set; }
        protected UserManager<ApplicationUser> UserManager { get; }

        protected InMemoryContextTestBase()
        {
            Context = ServiceProvider.GetService<AllReadyContext>();
            UserManager = ServiceProvider.GetService<UserManager<ApplicationUser>>();
        }
    }

    /// <summary>
    /// Inherit from this type to implement tests
    /// that make use of the in-memory test database
    /// context.
    /// </summary>
    public class InMemoryContextTest : InMemoryContextTestBase
    {
        protected InMemoryContextTest() : base()
        {
            LoadTestData();
        }

        /// <summary>
        /// Override this method to load test data
        /// into the in-memory database context prior
        /// to any tests being executed in your 
        /// test class.
        /// </summary>
        protected virtual void LoadTestData()
        {
        }
    }

    /// <summary>
    /// Inherit from this type if LoadTestData needs to await something
    /// </summary>
    public abstract class InMemoryContextTestAsync : InMemoryContextTestBase
    {
        protected InMemoryContextTestAsync() : base()
        {
            LoadTestData().Wait();
        }

        /// <summary>
        /// Override this method to load test data
        /// into the in-memory database context prior
        /// to any tests being executed in your 
        /// test class.
        /// !!!! Note: because a constructor can't be async,
        ///   it will block on the invocation of this method
        ///   which means any awaitable in LoadTestData must
        ///   include ConfigureAwait(false) to avoid deadlocks
        ///   http://blog.stephencleary.com/2012/07/dont-block-on-async-code.html
        /// </summary>
        protected virtual async Task LoadTestData()
        {
            await Task.Delay(0).ConfigureAwait(false); //To prevent compiler warning
        }
    }
}