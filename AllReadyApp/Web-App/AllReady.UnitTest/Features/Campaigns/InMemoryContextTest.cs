namespace AllReady.UnitTest.Features.Campaigns
{
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
}