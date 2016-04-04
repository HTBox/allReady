using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using AllReady.UnitTest.Features.Campaigns;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class DeleteCampaignCommandHandlerTests : InMemoryContextTest
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
            var command = new DeleteCampaignCommand { CampaignId = 1 };
            var handler = new DeleteCampaignCommandHandler(context);
            await handler.Handle(command);

            var data = context.Campaigns.Count(_ => _.Id == 1);
            Assert.Equal(0, data);
        }

        [Fact]
        public async Task CampaignDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var command = new DeleteCampaignCommand { CampaignId = 0 };
            var handler = new DeleteCampaignCommandHandler(context);
            await handler.Handle(command);
        }

        [Fact]
        public async Task CampaignIsDeleted()
        {
            const int campaignId = 1;

            var sut = new DeleteCampaignCommandHandler(Context);
            await sut.Handle(new DeleteCampaignCommand { CampaignId = campaignId });
            Assert.False(Context.Activities.Any(t => t.Id == campaignId));
        }

        [Fact]
        public async Task NonExistantTaskDoesNotCauseException()
        {
            const int campaignId = 1;

            var sut = new DeleteCampaignCommandHandler(Context);
            await sut.Handle(new DeleteCampaignCommand { CampaignId = 666 });
            Assert.False(Context.Activities.Any(t => t.Id == campaignId));
        }
    }
}