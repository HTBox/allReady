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
                new Event {
                    Id = unlockedEventId,
                    Location = new Location()
                    {
                        City = "Redmond",
                        State = "WA",
                        PostalCode = "98052",
                        Address1 = "7031 148th Ave Ne",
                        Country = "US"
                    },
                    Campaign = new Campaign
                    {
                        Locked = false,
                        ManagingOrganization = new Organization()
                        {
                            Name = "Humanitarian Toolbox"
                        }
                    }
                },
                new Event {Id = 2, Campaign = new Campaign {Locked = true, ManagingOrganization = new Organization()}}
            };
            Context.Events.AddRange(campaignEvents);
            Context.SaveChanges();

            var sut = new EventsWithUnlockedCampaignsQueryHandler(Context);
            var results = await sut.Handle(new EventsWithUnlockedCampaignsQuery());

            Assert.Equal(results[0].Id, unlockedEventId);
            Assert.Equal("98052", results[0].Location.PostalCode);
            Assert.Equal("Humanitarian Toolbox", results[0].OrganizationName);
        }
    }
}
