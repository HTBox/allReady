using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class DeleteCampaignCommandHandlerTests : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var htb = new Organization
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
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
        public async Task ExistingCampaign()
        {
            var command = new DeleteCampaignCommandAsync { CampaignId = 1 };
            var handler = new DeleteCampaignCommandHandlerAsync(Context);
            await handler.Handle(command);

            var data = Context.Campaigns.Count(_ => _.Id == 1);
            Assert.Equal(0, data);
        }

        [Fact]
        public async Task CampaignDoesNotExist()
        {
            var command = new DeleteCampaignCommandAsync { CampaignId = 0 };
            var handler = new DeleteCampaignCommandHandlerAsync(Context);
            await handler.Handle(command);
        }

        [Fact]
        public async Task CampaignIsDeleted()
        {
            const int campaignId = 1;

            var sut = new DeleteCampaignCommandHandlerAsync(Context);
            await sut.Handle(new DeleteCampaignCommandAsync { CampaignId = campaignId });
            Assert.False(Context.Events.Any(t => t.Id == campaignId));
        }

        [Fact]
        public async Task NonExistantTaskDoesNotCauseException()
        {
            const int campaignId = 1;

            var sut = new DeleteCampaignCommandHandlerAsync(Context);
            await sut.Handle(new DeleteCampaignCommandAsync() { CampaignId = 666 });
            Assert.False(Context.Events.Any(t => t.Id == campaignId));
        }
    }
}