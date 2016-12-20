using AllReady.Features.Home;
using AllReady.Models;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace AllReady.UnitTest.Features.Home
{
    public class ActiveOrUpcomingCampaignsQueryHandlerShould : InMemoryContextTest
    {
        private const string NotPublished = "Not published";
        private const string Expired = "Expired";
        private const string Locked = "Locked";

        private static DateTime DateTimeUtcTestDate = new DateTime(2016, 12, 01, 10, 00, 00, DateTimeKind.Utc);
        private static readonly DateTimeOffset DateTimeOffsetNow = new DateTimeOffset(DateTimeUtcTestDate);

        [Fact]
        public async Task ReturnCampaignsWhoseEndDateTimeIsGreaterThanOrEqualToToday()
        {
            // Arrange
            var handler = new ActiveOrUpcomingCampaignsQueryHandler(Context)
            {
                DateTimeOffsetUtcNow = () => DateTimeOffsetNow
            };

            // Act
            var result = await handler.Handle(new ActiveOrUpcomingCampaignsQuery());

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(4);
        }

        [Fact]
        public async Task NotIncludeAnyExpiredCampaigns()
        {
            // Arrange
            var handler = new ActiveOrUpcomingCampaignsQueryHandler(Context);

            // Act
            var result = await handler.Handle(new ActiveOrUpcomingCampaignsQuery());

            // Assert
            result.ShouldNotBeNull();
            result.Any(x => x.Name == Expired).ShouldBeFalse();
        }

        [Fact]
        public async Task NotIncludeAnyUnPublishedCampaigns()
        {
            // Arrange
            var handler = new ActiveOrUpcomingCampaignsQueryHandler(Context);

            // Act
            var result = await handler.Handle(new ActiveOrUpcomingCampaignsQuery());

            // Assert
            result.ShouldNotBeNull();
            result.Any(x => x.Name == NotPublished).ShouldBeFalse();
        }

        [Fact]
        public async Task NotIncludeAnyLockedCampaigns()
        {
            // Arrange
            var handler = new ActiveOrUpcomingCampaignsQueryHandler(Context);

            // Act
            var result = await handler.Handle(new ActiveOrUpcomingCampaignsQuery());

            // Assert
            result.ShouldNotBeNull();
            result.Any(x => x.Name == Locked).ShouldBeFalse();
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
                Name = "This is published and in the future",
                Featured = false,
                ManagingOrganization = org,
                Published = true,
                StartDateTime = DateTimeUtcTestDate.AddDays(1),
                EndDateTime = DateTimeUtcTestDate.AddDays(10)
            });

            Context.Campaigns.Add(new Campaign
            {
                Name = "This is also published and in the future",
                Featured = false,
                ManagingOrganization = org,
                Published = true,
                StartDateTime = DateTimeUtcTestDate.AddDays(1),
                EndDateTime = DateTimeUtcTestDate.AddDays(10)
            });

            Context.Campaigns.Add(new Campaign
            {
                Name = NotPublished,
                Featured = false,
                ManagingOrganization = org,
                Published = false,
                StartDateTime = DateTimeUtcTestDate.AddDays(1),
                EndDateTime = DateTimeUtcTestDate.AddDays(10)
            });

            Context.Campaigns.Add(new Campaign
            {
                Name = "This is published and started",
                Featured = false,
                ManagingOrganization = org,
                Published = true,
                StartDateTime = DateTimeUtcTestDate.AddDays(-1),
                EndDateTime = DateTimeUtcTestDate.AddDays(10)
            });

            Context.Campaigns.Add(new Campaign
            {
                Name = Expired,
                Featured = false,
                ManagingOrganization = org,
                Published = true,
                StartDateTime = DateTimeUtcTestDate.AddDays(-10),
                EndDateTime = DateTimeUtcTestDate.AddDays(-1)
            });

            Context.Campaigns.Add(new Campaign
            {
                Name = "This is published and active for the test date",
                Featured = false,
                ManagingOrganization = org,
                Published = true,
                StartDateTime = new DateTime(2016, 12, 01, 00, 00, 00, DateTimeKind.Utc),
                EndDateTime = new DateTime(2016, 12, 01, 23, 59, 59, DateTimeKind.Utc)
            });

            Context.Campaigns.Add(new Campaign
            {
                Name = Expired, //one minute before the current date
                Featured = false,
                ManagingOrganization = org,
                Published = true,
                StartDateTime = DateTimeUtcTestDate.AddDays(-10),
                EndDateTime = new DateTime(2016, 11, 30, 23, 59, 59)
        });

            Context.Campaigns.Add(new Campaign
            {
                Name = Locked,
                Featured = false,
                ManagingOrganization = org,
                Published = true,
                StartDateTime = DateTimeUtcTestDate.AddDays(-2),
                EndDateTime = DateTimeUtcTestDate.AddDays(5),
                Locked = true
            });

            Context.SaveChanges();
        }
    }
}
