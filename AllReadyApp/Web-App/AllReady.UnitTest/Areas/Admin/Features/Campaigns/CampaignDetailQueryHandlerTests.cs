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
        protected override void LoadTestData()
        {
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
            Context.Organizations.Add(htb);
            Context.SaveChanges();
        }

        [Fact]
        public async Task CampaignExists()
        {
            var query = new CampaignDetailQuery { CampaignId = 1 };
            var handler = new CampaignDetailQueryHandler(Context);
            var result = await handler.Handle(query);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CampaignDoesNotExist()
        {
            var query = new CampaignDetailQuery { CampaignId = 0 };
            var handler = new CampaignDetailQueryHandler(Context);
            var result = await handler.Handle(query);
            Assert.Null(result);
        }
    }
}