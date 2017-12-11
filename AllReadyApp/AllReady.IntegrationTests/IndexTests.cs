using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.IntegrationTests
{
    public class IndexTests : IClassFixture<TestFixture<IntegrationTestStartup>>
    { 
        private readonly HttpClient _client;

        public IndexTests(TestFixture<IntegrationTestStartup> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task CanGetHomePage()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
