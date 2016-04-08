using System;
using System.Collections.Generic;
using AllReady.Areas.Admin.Features.Activities;
using AllReady.Models;
using AllReady.UnitTest.Features.Campaigns;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Activities
{
    public class ActivityDetailQueryHandlerTests : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();

            PostalCodeGeo seattlePostalCode = new PostalCodeGeo() { City = "Seattle", PostalCode = "98117", State = "WA" };
            Location seattle = new Location()
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
         

            Organization htb = new Organization()
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

            var queenAnne = new Activity
            {
                Id = 1,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<ActivitySkill>()
            };
            context.PostalCodes.Add(seattlePostalCode);
            context.Locations.Add(seattle);
            context.Organizations.Add(htb);
            context.Activities.Add(queenAnne);
            context.SaveChanges();
        }

        [Fact]
        public void ActivityExists()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new ActivityDetailQuery { ActivityId = 1 };
            var handler = new ActivityDetailQueryHandler(context);
            var result = handler.Handle(query);
            Assert.NotNull(result);
        }

        [Fact]
        public void ActivityDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new ActivityDetailQuery();
            var handler = new ActivityDetailQueryHandler(context);
            var result = handler.Handle(query);
            Assert.Null(result);
        }

        [Fact]
        public void ActivityIncludesAllLocationInformation()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new ActivityDetailQuery() { ActivityId = 1 };
            var handler = new ActivityDetailQueryHandler(context);
            var result = handler.Handle(query);
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
