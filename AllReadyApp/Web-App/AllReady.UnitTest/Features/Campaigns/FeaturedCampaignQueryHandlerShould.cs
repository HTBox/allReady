using System;
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
        public async Task ReturnASingleCampaignThatIsFeaturedAndHasNotEnded()
        {
            // Arrange
            var handler = new FeaturedCampaignQueryHandler(Context)
            {
                DateTimeOffsetUtcNow = () => DateTime.Now
            };

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

        [Fact]
        public async Task ReturnNullIfFeaturedCampaignIsNotMarkedAsPublished()
        {
            // clear the test data of all campaigns
            var allCampaigns = Context.Campaigns.ToList();
            Context.RemoveRange(allCampaigns);
            Context.SaveChanges();

            var org = new Organization
            {
                Name = "Some Organization"
            };

            Context.Campaigns.Add(new Campaign
            {
                Name = "This is featured but not published",
                Featured = true,
                ManagingOrganization = org,
                Published = false,
                EndDateTime = new DateTime(2018, 1, 1)
            });
            
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
                Id = 1,
                Name = "This is featured but has ended",
                Featured = true,
                ManagingOrganization = org,
                Published = true,
                EndDateTime = DateTime.Now.AddDays(-10)
            });

            Context.Campaigns.Add(new Campaign
            {
                Id = 2,
                Name = "This is featured",
                Featured = true,
                ManagingOrganization = org,
                Published = true,
                EndDateTime = DateTime.Now.AddDays(90) // future date
            });

            Context.Campaigns.Add(new Campaign
            {
                Id = 3,
                Name = "This is not featured",
                Featured = false,
                ManagingOrganization = org,
                Published = true,
                EndDateTime = DateTime.Now.AddDays(90) // future date
            });

            Context.Campaigns.Add(new Campaign
            {
                Id = 4,
                Name = "This is also featured",
                Featured = true,
                ManagingOrganization = org,
                Published = true,
                EndDateTime = DateTime.Now.AddDays(90) // future date
            });

            Context.SaveChanges();
        }
    }
}
