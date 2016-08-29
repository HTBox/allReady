using System.Collections.Generic;
using AllReady.Features.Event;
using AllReady.Models;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class EventsWithUnlockedCampaignsQueryHandlerTests : InMemoryContextTest
    {
        [Fact]
        public async Task HandleReturnsEventsWitUnlockedCampaigns()
        {
            var options = this.CreateNewContextOptions();

            const int unlockedEventId = 1;

            using (var context = new AllReadyContext(options)) {
                var campaignEvents = new List<Models.Event> {
                    new Models.Event {Id = unlockedEventId, Campaign = new Campaign {Locked = false, ManagingOrganization = new Organization()}},
                    new Models.Event {Id = 2, Campaign = new Campaign {Locked = true, ManagingOrganization = new Organization()}}
                };
                context.Events.AddRange(campaignEvents);
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new EventsWithUnlockedCampaignsQueryHandler(context);
                var results = sut.Handle(new EventsWithUnlockedCampaignsQuery());

                Assert.Equal(results[0].Id, unlockedEventId);
            }
        }
    }
}
