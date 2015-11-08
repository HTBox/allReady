using AllReady.Areas.Admin.Features.Activities;
using AllReady.Areas.Admin.ViewModels;
using AllReady.Models;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest
{
    public class EditActivity : TestBase
    {
        [Fact]
        public void ActivityDoesNotExist()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var context = ServiceProvider.GetService<AllReadyContext>();
                var vm = new ActivityDetailViewModel
                {
                    Id = 0
                };
                var query = new EditActivityCommand { Activity = vm };
                var handler = new EditActivityCommandHandler(context);
                var result = handler.Handle(query);
            });
        }

        [Fact]
        public void ExistingActivity()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            Tenant htb = new Tenant()
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };
            Campaign firePrev = new Campaign()
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingTenant = htb
            };
            htb.Campaigns.Add(firePrev);
            Activity queenAnne = new Activity()
            {
                Id = 1,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTimeUtc = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTimeUtc = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                Tenant = htb,
                RequiredSkills = new List<ActivitySkill>()
            };
            context.Tenants.Add(htb);
            context.Activities.Add(queenAnne);
            context.SaveChanges();

            const string NEW_NAME = "Some new name value";

            var vm = new ActivityDetailViewModel
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
                TenantId = queenAnne.TenantId,
                TenantName = queenAnne.Tenant.Name,
                Volunteers = null
            };
            var query = new EditActivityCommand { Activity = vm };
            var handler = new EditActivityCommandHandler(context);
            var result = handler.Handle(query);
            Assert.Equal(1, result); // should get back the activity id

            var data = context.Activities.Single(_ => _.Id == 1);
            Assert.Equal(NEW_NAME, data.Name);
        }

    }
}
