using System.Collections.Generic;
using AllReady.Features.Event;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class EventsWithUnlockedCampaignsQueryHandlerTests
    {
        [Fact]
        public void HandleReturnsEventsWitUnlockedCampaigns()
        {
            var campaignEvents = new List<Models.Event>
            {
                new Models.Event { Id = 1, Campaign = new Campaign { Locked = false, ManagingOrganization = new Organization() }},
                new Models.Event { Id = 2, Campaign = new Campaign { Locked = true, ManagingOrganization = new Organization() }}
            };

            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.Events).Returns(campaignEvents);

            var sut = new EventsWithUnlockedCampaignsQueryHandler(dataAccess.Object);
            var results = sut.Handle(new EventsWithUnlockedCampaignsQuery());

            Assert.Equal(campaignEvents[0].Id, results[0].Id);
        }
    }
}
