using System.Collections.Generic;
using AllReady.Models;
using System.Threading.Tasks;
using AllReady.Features.Events;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    using Event = AllReady.Models.Event;

    public class EventsWithUnlockedCampaignsQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task HandleReturnsEventsWitUnlockedCampaigns()
        {
            const int unlockedEventId = 1;

            var campaignEvents = new List<Event>
            {
                new Event {Id = unlockedEventId, Campaign = new Campaign {Locked = false, ManagingOrganization = new Organization()}},
                new Event {Id = 2, Campaign = new Campaign {Locked = true, ManagingOrganization = new Organization()}}
            };
            Context.Events.AddRange(campaignEvents);
            Context.SaveChanges();

            var sut = new EventsWithUnlockedCampaignsQueryHandler(Context);
            var results = await sut.Handle(new EventsWithUnlockedCampaignsQuery());

            Assert.Equal(results[0].Id, unlockedEventId);
        }
    }
}
