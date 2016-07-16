using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Areas.Admin.Models.ItineraryModels;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Itineraries
{
    public class EditItineraryCommandHandlerAsyncTests : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var htb = new Organization
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            var firePrev = new Campaign
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb
            };
            htb.Campaigns.Add(firePrev);

            var queenAnne = new Event
            {
                Id = 1,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<EventSkill>(),
                EventType = EventType.Itinerary
            };

            var itinerary = new Itinerary
            {
                Event = queenAnne,
                Name = "1st Itinerary",
                Date = new DateTime(2016, 07, 01)
            };

            Context.Events.Add(queenAnne);
            Context.Itineraries.Add(itinerary);
            Context.SaveChanges();
        }

        [Fact]
        public async Task AddsNewItineraryWhenItDoesNotExist()
        {
            var query = new EditItineraryCommand {  Itinerary = new ItineraryEditModel
            {
                EventId = 1,
                Name = "New",
                Date = DateTime.Now
            }};

            var sut = new EditItineraryCommandHandlerAsync(Context);
            var result = await sut.Handle(query);

            Assert.True(result > 0);
            Assert.Equal(2, Context.Itineraries.Count());

            var data = Context.Itineraries.Count(x => x.Id == result);

            Assert.True(data == 1);
        }

        [Fact]
        public async Task UpdatesItineraryWhenItExists()
        {
            var query = new EditItineraryCommand
            {
                Itinerary = new ItineraryEditModel
                {
                    Id = 1,
                    EventId = 1,
                    Name = "Updated",
                    Date = DateTime.Now
                }
            };

            var sut = new EditItineraryCommandHandlerAsync(Context);
            var result = await sut.Handle(query);

            Assert.True(result == 1);
            Assert.Equal(1, Context.Itineraries.Count());
        }
    }
}
