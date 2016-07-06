using AllReady.Features.Campaigns;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Campaigns
{
    public class FeaturedCampaignQueryHandlerAsyncTests : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnsSingleCampaignThatIsFeatured()
        {
            // Arrange
            var handler = new FeaturedCampaignQueryHandlerAsync(Context);            

            // Act
            var result = await handler.Handle(new FeaturedCampaignQueryAsync());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("This is featured", result.Title);            
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task FeaturedCampaignIncludesOrg()
        {
            // Arrange
            var handler = new FeaturedCampaignQueryHandlerAsync(Context);

            // Act
            var result = await handler.Handle(new FeaturedCampaignQueryAsync());

            // Assert
            Assert.NotNull(result.Organization);
            Assert.Equal("Some Organization", result.Organization.Name);
        }

        [Fact]
        public async Task ReturnNullIfNoCampaignsAreFeatured()
        {
            var results = Context.Campaigns.Where(x => x.Featured);
            Context.RemoveRange(results);
            Context.SaveChanges();

            // Arrange
            var handler = new FeaturedCampaignQueryHandlerAsync(Context);

            // Act
            var result = await handler.Handle(new FeaturedCampaignQueryAsync());

            // Assert
            Assert.Null(result);
        }

        protected override void LoadTestData()
        {
            var org = new Models.Organization
            {
                Name = "Some Organization"
            };

            Context.Organizations.Add(org);

            Context.Campaigns.Add(new Models.Campaign
            {
                Name = "This is featured",
                Featured = true,
                ManagingOrganization = org                
            });

            Context.Campaigns.Add(new Models.Campaign
            {
                Name = "This is not featured",
                Featured = false,
                ManagingOrganization = org
            });

            Context.Campaigns.Add(new Models.Campaign
            {
                Name = "This is also featured",
                Featured = true,
                ManagingOrganization = org
            });

            Context.SaveChanges();
        }
    }
}