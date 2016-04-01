using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Campaigns
{
    public class CampaignDetailQueryHandlerTests : InMemoryContextTest
    {
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
                Id = 1,
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb
            };
            htb.Campaigns.Add(firePrev);
            context.Organizations.Add(htb);
            context.SaveChanges();
        }

        [Fact]
        public async Task CampaignExists()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new CampaignDetailQuery { CampaignId = 1 };
            var handler = new CampaignDetailQueryHandler(context);
            var result = await handler.Handle(query);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CampaignDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new CampaignDetailQuery { CampaignId = 0 };
            var handler = new CampaignDetailQueryHandler(context);
            var result = await handler.Handle(query);
            Assert.Null(result);
        }
    }
}