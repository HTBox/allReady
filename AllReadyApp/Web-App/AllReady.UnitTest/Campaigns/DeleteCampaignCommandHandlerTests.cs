using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Campaigns
{
    public class DeleteCampaignCommandHandlerTests : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            Organization htb = new Organization()
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
                ManagingOrganization = htb
            };
            htb.Campaigns.Add(firePrev);
            context.Organizations.Add(htb);
            context.SaveChanges();
        }

        [Fact]
        public async Task ExistingCampaign()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new DeleteCampaignCommand { CampaignId = 1 };
            var handler = new DeleteCampaignCommandHandler(context);
            await handler.Handle(query);

            var data = context.Campaigns.Count(_ => _.Id == 1);
            Assert.Equal(0, data);
        }

        [Fact]
        public async Task CampaignDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new DeleteCampaignCommand { CampaignId = 0 };
            var handler = new DeleteCampaignCommandHandler(context);
            await handler.Handle(query);
        }
    }
}
