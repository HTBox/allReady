using AllReady.Features.Home;
using AllReady.Models;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Home
{
    public class ActiveOrUpcomingCampaignsQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task NotReturnUnPublishedCampaigns()
        {
            // Arrange
            var handler = new ActiveOrUpcomingCampaignsQueryHandler(Context);

            // Act
            var result = await handler.Handle(new ActiveOrUpcomingCampaignsQuery());

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
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
                Name = "This is published",
                Featured = false,
                ManagingOrganization = org,
                Published = true,
                StartDateTime = DateTime.UtcNow.AddDays(1),
                EndDateTime = DateTime.UtcNow.AddDays(10)
            });

            Context.Campaigns.Add(new Campaign
            {
                Name = "This is also published",
                Featured = false,
                ManagingOrganization = org,
                Published = true,
                StartDateTime = DateTime.UtcNow.AddDays(1),
                EndDateTime = DateTime.UtcNow.AddDays(10)
            });

            Context.Campaigns.Add(new Campaign
            {
                Name = "This is not published",
                Featured = false,
                ManagingOrganization = org,
                Published = false,
                StartDateTime = DateTime.UtcNow.AddDays(1),
                EndDateTime = DateTime.UtcNow.AddDays(10)
            });

            Context.SaveChanges();
        }
    }
}
