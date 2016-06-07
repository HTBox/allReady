using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Itineraries
{
    public class ItineraryDetailQueryHandlerAsyncTests : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();

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
                Id = 1,
                Date = new DateTime(2016, 07, 01)
            };

            context.Events.Add(queenAnne);
            context.Itineraries.Add(itinerary);
            context.SaveChanges();
        }

        // TODO - Create Tests to verify query behaves as expected and returns correct data
    }
}
