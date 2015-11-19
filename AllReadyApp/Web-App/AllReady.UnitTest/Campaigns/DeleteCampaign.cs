using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllReady.UnitTest.Campaigns
{
    public class DeleteCampaign : InMemoryContextTest
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
        public void ExistingCampaign()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new DeleteCampaignCommand { CampaignId = 1 };
            var handler = new DeleteCampaignCommandHandler(context);
            var result = handler.Handle(query);

            var data = context.Campaigns.Count(_ => _.Id == 1);
            Assert.Equal(0, data);
        }

        [Fact]
        public void CampaignDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new DeleteCampaignCommand { CampaignId = 0 };
            var handler = new DeleteCampaignCommandHandler(context);
            var result = handler.Handle(query);
        }
    }
}
