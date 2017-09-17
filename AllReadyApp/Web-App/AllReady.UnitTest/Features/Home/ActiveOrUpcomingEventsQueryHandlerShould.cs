using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Home;
using AllReady.Models;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Features.Home
{
    public class ActiveOrUpcomingEventsQueryHandlerShould : InMemoryContextTest
    {
        private const string Expired = "Expired";
        private const string EventForLockedCampaign = "Event for locked campaign";
        private const string EventForNotPublishedCampaign = "Event for not published campaign";

        private static DateTime DateTimeUtcTestDate = new DateTime(2016, 12, 01, 10, 00, 00, DateTimeKind.Utc);
        private static readonly DateTimeOffset DateTimeOffsetNow = new DateTimeOffset(DateTimeUtcTestDate);

        [Fact]
        public async Task ReturnEventsWhoseEndDateTimeIsGreaterThanOrEqualToToday()
        {
            // Arrange
            var handler = new ActiveOrUpcomingEventsQueryHandler(Context)
            {
                DateTimeOffsetUtcNow = () => DateTimeOffsetNow
            };

            // Act
            var result = await handler.Handle(new ActiveOrUpcomingEventsQuery());

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(4);
        }

        [Fact]
        public async Task NotIncludeAnyExpiredEvents()
        {
            // Arrange
            var handler = new ActiveOrUpcomingEventsQueryHandler(Context);

            // Act
            var result = await handler.Handle(new ActiveOrUpcomingEventsQuery());

            // Assert
            result.ShouldNotBeNull();
            result.Any(x => x.Name == Expired).ShouldBeFalse();
        }

        [Fact]
        public async Task NotReturnEventsForLockedCampaigns()
        {
            // Arrange
            var handler = new ActiveOrUpcomingEventsQueryHandler(Context);

            // Act
            var result = await handler.Handle(new ActiveOrUpcomingEventsQuery());

            // Assert
            result.ShouldNotBeNull();
            result.Any(x => x.Name == EventForLockedCampaign).ShouldBeFalse();
        }

        [Fact]
        public async Task NotReturnEventsForNotPublishedCampaigns()
        {
            // Arrange
            var handler = new ActiveOrUpcomingEventsQueryHandler(Context);

            // Act
            var result = await handler.Handle(new ActiveOrUpcomingEventsQuery());

            // Assert
            result.ShouldNotBeNull();
            result.Any(x => x.Name == EventForNotPublishedCampaign).ShouldBeFalse();
        }

        protected override void LoadTestData()
        {
            var managingOrganization = new Organization
            {
                Name = "Some Managing Organization Name"
            };
        
            var campaign = new Campaign()
            {
                Name = "Some Campaign",
                ManagingOrganization = managingOrganization,
                Published = true,
            };

            var lockedCampaign = new Campaign
            {
                Name = "Locked campaign",
                ManagingOrganization = managingOrganization,
                Published = true,
                Locked = true,
            };

            var notPublishedCampaign = new Campaign
            {
                Name = "Not published campaign",
                ManagingOrganization = managingOrganization,
                Published = false
            };
            
            Context.Events.Add(new AllReady.Models.Event
            {
                Name = "This is in the future",
                Campaign = campaign,
                StartDateTime = DateTimeUtcTestDate.AddDays(1),
                EndDateTime = DateTimeUtcTestDate.AddDays(10)
            });

            Context.Events.Add(new AllReady.Models.Event
            {
                Name = "This is also  in the future",
                Campaign = campaign,
                StartDateTime = DateTimeUtcTestDate.AddDays(1),
                EndDateTime = DateTimeUtcTestDate.AddDays(10)
            });
            
            Context.Events.Add(new AllReady.Models.Event
            {
                Name = "This is event has started",
                Campaign = campaign,
                StartDateTime = DateTimeUtcTestDate.AddDays(-1),
                EndDateTime = DateTimeUtcTestDate.AddDays(10)
            });

            Context.Events.Add(new AllReady.Models.Event
            {
                Name = Expired,
                Campaign = campaign,
                StartDateTime = DateTimeUtcTestDate.AddDays(-10),
                EndDateTime = DateTimeUtcTestDate.AddDays(-1)
            });

            Context.Events.Add(new AllReady.Models.Event
            {
                Name = "This is active for the test date",
                Campaign = campaign,
                StartDateTime = new DateTime(2016, 12, 01, 00, 00, 00, DateTimeKind.Utc),
                EndDateTime = new DateTime(2016, 12, 01, 23, 59, 59, DateTimeKind.Utc)
            });

            Context.Events.Add(new AllReady.Models.Event
            {
                Name = Expired, //one minute before the current date
                Campaign = campaign,
                StartDateTime = DateTimeUtcTestDate.AddDays(-10),
                EndDateTime = new DateTime(2016, 11, 30, 23, 59, 59)
            });

            Context.Events.Add(new AllReady.Models.Event
            {
                Name = EventForLockedCampaign,
                Campaign = lockedCampaign,
                StartDateTime = DateTimeUtcTestDate.AddDays(1),
                EndDateTime = DateTimeUtcTestDate.AddDays(10)
            });

            Context.Events.Add(new AllReady.Models.Event
            {
                Name = EventForNotPublishedCampaign,
                Campaign = notPublishedCampaign,
                StartDateTime = DateTimeUtcTestDate.AddDays(1),
                EndDateTime = DateTimeUtcTestDate.AddDays(10)
            });

            Context.SaveChanges();
        }
    }
}