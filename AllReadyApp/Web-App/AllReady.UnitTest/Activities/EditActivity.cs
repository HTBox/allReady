using AllReady.Areas.Admin.Features.Activities;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllReady.UnitTest.Activities
{
    public class EditActivity : TestBase
    {
        [Fact]
        public void ActivityDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            Tenant htb = new Tenant()
            {
                Id = 123,
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };
            Campaign firePrev = new Campaign()
            {
                Id = 1,
                Name = "Neighborhood Fire Prevention Days",
                ManagingTenant = htb
            };
            htb.Campaigns.Add(firePrev);
            context.Tenants.Add(htb);
            context.SaveChanges();

            var vm = new ActivityDetailModel
            {
                CampaignId = 1 
            };
            var query = new EditActivityCommand { Activity = vm };
            var handler = new EditActivityCommandHandler(context);
            var result = handler.Handle(query);
            Assert.True(result > 0);

            var data = context.Activities.Count(_ => _.Id == result);
            Assert.True(data == 1);
        }

        [Fact]
        public void ExistingActivity()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            Tenant htb = new Tenant()
            {
                Id = 123,
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };
            Campaign firePrev = new Campaign()
            {
                Id = 1,
                Name = "Neighborhood Fire Prevention Days",
                ManagingTenant = htb
            };
            htb.Campaigns.Add(firePrev);
            Activity queenAnne = new Activity()
            {
                Id = 100,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTimeUtc = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTimeUtc = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<ActivitySkill>()
            };
            context.Tenants.Add(htb);
            context.Activities.Add(queenAnne);
            context.SaveChanges();

            const string NEW_NAME = "Some new name value";

            var vm = new ActivityDetailModel
            {
                CampaignId = queenAnne.CampaignId,
                CampaignName = queenAnne.Campaign.Name,
                Description = queenAnne.Description,
                EndDateTime = queenAnne.EndDateTimeUtc,
                Id = queenAnne.Id,
                ImageUrl = queenAnne.ImageUrl,
                Location = null,
                Name = NEW_NAME,
                RequiredSkills = queenAnne.RequiredSkills,
                StartDateTime = queenAnne.StartDateTimeUtc,
                Tasks = null,
                TenantId = queenAnne.Campaign.ManagingTenantId,
                TenantName = queenAnne.Campaign.ManagingTenant.Name,
                Volunteers = null
            };
            var query = new EditActivityCommand { Activity = vm };
            var handler = new EditActivityCommandHandler(context);
            var result = handler.Handle(query);
            Assert.Equal(100, result); // should get back the activity id

            var data = context.Activities.Single(_ => _.Id == result);
            Assert.Equal(NEW_NAME, data.Name);
        }

    }
}
