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
    }
}