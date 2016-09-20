using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.UnitTest.Features.Campaigns;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class CampaignDetailQueryHandlerTests : InMemoryContextTest
    {
        private int _campaignId;

        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();

            var htb = new Organization
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>(),
            };

            var firePrev = new Campaign
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb
            };
            htb.Campaigns.Add(firePrev);
            context.Organizations.Add(htb);
            context.SaveChanges();

            _campaignId = firePrev.Id;
        }

        [Fact]
        public async Task CampaignExists()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new CampaignDetailQueryAsync { CampaignId = _campaignId };
            var handler = new CampaignDetailQueryHandlerAsync(context);
            var result = await handler.Handle(query);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CampaignDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new CampaignDetailQueryAsync { CampaignId = 0 };
            var handler = new CampaignDetailQueryHandlerAsync(context);
            var result = await handler.Handle(query);
            Assert.Null(result);
        }
    }
}