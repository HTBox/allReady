using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class EventDetailQueryHandlerTests : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();

            var seattlePostalCode = new PostalCodeGeo { City = "Seattle", PostalCode = "98117", State = "WA" };
            var seattle = new Location
            {
                Id = 1,
                Address1 = "123 Main Street",
                Address2 = "Unit 2",
                City = "Seattle",
                PostalCode = seattlePostalCode,
                Country = "USA",
                State = "WA",
                Name = "Organizer name",
                PhoneNumber = "555-555-5555"                            
            };
         

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
                RequiredSkills = new List<EventSkill>()
            };

            context.PostalCodes.Add(seattlePostalCode);
            context.Locations.Add(seattle);
            context.Organizations.Add(htb);
            context.Events.Add(queenAnne);
            context.SaveChanges();
        }

        [Fact]
        public async Task EventExists()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new EventDetailQuery { EventId = 1 };
            var handler = new EventDetailQueryHandler(context);
            var result = await handler.Handle(query);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task EventDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new EventDetailQuery();
            var handler = new EventDetailQueryHandler(context);
            var result = await handler.Handle(query);
            Assert.Null(result);
        }

        [Fact]
        public async Task EventIncludesAllLocationInformation()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new EventDetailQuery { EventId = 1 };
            var handler = new EventDetailQueryHandler(context);
            var result = await handler.Handle(query);

            Assert.NotNull(result.Location);
            Assert.NotNull(result.Location?.Id);
            Assert.NotNull(result.Location?.Address1);
            Assert.NotNull(result.Location?.Address2);
            Assert.NotNull(result.Location?.PostalCode);
            Assert.NotNull(result.Location?.State);
            Assert.NotNull(result.Location?.Name);
            Assert.NotNull(result.Location?.PhoneNumber);
            Assert.NotNull(result.Location?.Country);
        }
    }
}
