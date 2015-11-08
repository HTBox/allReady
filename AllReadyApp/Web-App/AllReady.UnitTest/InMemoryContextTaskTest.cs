using AllReady.Models;
using AllReady.UnitTest;
using Microsoft.Framework.DependencyInjection;

namespace AllReady.UnitTest
{
    /// <summary>
    /// Inherit from this type to implement tests
    /// that make use of the in-memory test database
    /// context.
    /// </summary>
    public abstract class InMemoryContextTest : TestBase
    {
        /// <summary>
        /// Gets the in-memory database context.
        /// </summary>
        protected AllReadyContext Context { get; private set; }

        protected InMemoryContextTest()
        {
            Context = ServiceProvider.GetService<AllReadyContext>();
            LoadTestData();
        }

        /// <summary>
        /// Override this method to load test data
        /// into the in-memory database context prior
        /// to any tests being executed in your 
        /// test class.
        /// </summary>
        protected virtual void LoadTestData()
        { }

    }
}