using AllReady.Features.Campaigns;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Campaigns
{
    public class FeaturedCampaignQueryHandlerTests : InMemoryContextTest
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