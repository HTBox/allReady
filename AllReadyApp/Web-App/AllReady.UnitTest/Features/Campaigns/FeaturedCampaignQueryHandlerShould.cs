using AllReady.Features.Campaigns;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Features.Campaigns
{
    public class FeaturedCampaignQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnsSingleCampaignThatIsFeatured()
        {
            // Arrange
            var handler = new FeaturedCampaignQueryHandler(Context);            

            // Act
            var result = await handler.Handle(new FeaturedCampaignQuery());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("This is featured", result.Title);            
        }

        [Fact]
        public async Task FeaturedCampaignIncludesOrg()
        {
            // Arrange
            var handler = new FeaturedCampaignQueryHandler(Context);

            // Act
            var result = await handler.Handle(new FeaturedCampaignQuery());

            // Assert
            Assert.NotNull(result.OrganizationName);
            Assert.Equal("Some Organization", result.OrganizationName);
        }

        [Fact]
        public async Task ReturnNullIfNoCampaignsAreFeatured()
        {
            var results = Context.Campaigns.Where(x => x.Featured);
            Context.RemoveRange(results);
            Context.SaveChanges();

            // Arrange
            var handler = new FeaturedCampaignQueryHandler(Context);

            // Act
            var result = await handler.Handle(new FeaturedCampaignQuery());

            // Assert
            Assert.Null(result);
        }

        protected override void LoadTestData()
        {
            var org = new Organization
            {
                Name = "Some Organization"
            };

            Context.Organizations.Add(org);

            Context.Campaigns.Add(new Campaign
            {
                Name = "This is featured",
                Featured = true,
                ManagingOrganization = org,
                Published = true         
            });

            Context.Campaigns.Add(new Campaign
            {
                Name = "This is not featured",
                Featured = false,
                ManagingOrganization = org,
                Published = true
            });

            Context.Campaigns.Add(new Campaign
            {
                Name = "This is also featured",
                Featured = true,
                ManagingOrganization = org,
                Published = true
            });

            Context.SaveChanges();
        }
    }
}