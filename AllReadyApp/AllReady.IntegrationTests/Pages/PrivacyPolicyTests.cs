using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace AllReady.IntegrationTests.Pages
{
    public class PrivacyPolicyTests : IClassFixture<TestFixture<IntegrationTestStartup>>
    {
        private readonly HttpClient _client;

        public PrivacyPolicyTests(TestFixture<IntegrationTestStartup> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task CanGetAboutPage()
        {
            // Act
            var response = await _client.GetAsync("/privacypolicy");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}
