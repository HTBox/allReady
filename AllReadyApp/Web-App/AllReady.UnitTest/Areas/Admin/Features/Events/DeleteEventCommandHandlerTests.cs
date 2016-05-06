using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Models;
using AllReady.UnitTest.Features.Campaigns;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class DeleteEventCommandHandlerTests : InMemoryContextTest
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
                RequiredSkills = new List<EventSkill>()
            };

            context.Organizations.Add(htb);
            context.Events.Add(queenAnne);
            context.SaveChanges();
        }

        [Fact]
        public void ExistingEvent()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var command = new DeleteEventCommand { EventId = 1 };
            var handler = new DeleteEventCommandHandler(context);
            handler.Handle(command);

            var data = context.Events.Count(_ => _.Id == 1);
            Assert.Equal(0, data);
        }

        [Fact]
        public void EventDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var command = new DeleteEventCommand { EventId = 0 };
            var handler = new DeleteEventCommandHandler(context);
            handler.Handle(command);
            //TODO: this test needs to be completed to actually test something
        }

        [Fact]
        public void EventIsDeleted()
        {
            const int eventId = 1;
            var sut = new DeleteEventCommandHandler(Context);
            sut.Handle(new DeleteEventCommand { EventId = eventId });
            Assert.False(Context.Events.Any(t => t.Id == eventId));
        }
    }
}