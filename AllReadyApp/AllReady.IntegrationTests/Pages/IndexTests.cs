using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AllReady.Models;
using Shouldly;
using Xunit;

namespace AllReady.IntegrationTests.Pages
{
    public class IndexTests : IClassFixture<TestFixture<IntegrationTestStartup>>
    { 
        private readonly HttpClient _client;

        private const string CampaignName = "Featured Campaign Name";

        public IndexTests(TestFixture<IntegrationTestStartup> fixture)
        {
            _client = fixture.Client;
           
            var campaign = new Campaign
            {
                EndDateTime = DateTimeOffset.UtcNow.AddDays(10),
                Featured = true,
                Published = true,
                Locked = false,
                Name = CampaignName,
                Description = "This is a featured campaign",
                Headline = "This is a featured headline",
                ManagingOrganization = new Organization
                {
                    Name = "Test Organisation"
                }
            };

            fixture.DbContext.Campaigns.Add(campaign);

            fixture.DbContext.Events.Add(new Event { Campaign = campaign, Name = "Event Name 1", EndDateTime = DateTimeOffset.UtcNow.AddDays(2) });
            fixture.DbContext.Events.Add(new Event { Campaign = campaign, Name = "Event Name 2", EndDateTime = DateTimeOffset.UtcNow.AddDays(2) });

            fixture.DbContext.SaveChanges();
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
            content.ShouldContain(CampaignName);
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
