using AllReady.Areas.Admin.Features.Campaigns;
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

namespace AllReady.UnitTest.Campaigns
{
    public class GetCampaignDetail : InMemoryContextTest
    {
        protected override void LoadTestData()
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
                Id = 1,
                Name = "Neighborhood Fire Prevention Days",
                ManagingTenant = htb
            };
            htb.Campaigns.Add(firePrev);
            context.Tenants.Add(htb);
            context.SaveChanges();
        }

        [Fact]
        public void CampaignExists()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new CampaignDetailQuery { CampaignId = 1 };
            var handler = new CampaignDetailQueryHandler(context);
            var result = handler.Handle(query);
            Assert.NotNull(result);
        }

        [Fact]
        public void CampaignDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new CampaignDetailQuery { CampaignId = 0 };
            var handler = new CampaignDetailQueryHandler(context);
            var result = handler.Handle(query);
            Assert.Null(result);
        }
    }
}
