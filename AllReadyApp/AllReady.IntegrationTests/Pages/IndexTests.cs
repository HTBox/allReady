using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace AllReady.IntegrationTests.Pages
{
    public class IndexTests : IClassFixture<TestFixture<IntegrationTestStartup>>
    { 
        private readonly HttpClient _client;

        private const string FeaturedCampaignName = "Featured Campaign Name";

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
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task IncludesFeaturedCampaign_WhenSet()
        {
            // Act
            var response = await _client.GetAsync("/");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.ShouldContain(FeaturedCampaignName);
        }

        [Fact]
        public async Task IncludesExpectedNumberOfEvents()
        {
            // Act
            var response = await _client.GetAsync("/");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            var newLineFreeContent = Regex.Replace(content, @"\t|\n|\r|\s+", string.Empty);
            Regex.Matches(newLineFreeContent, "</h4><p><b>Campaign:</b>").Count.ShouldBe(2);
        }
    }
}
