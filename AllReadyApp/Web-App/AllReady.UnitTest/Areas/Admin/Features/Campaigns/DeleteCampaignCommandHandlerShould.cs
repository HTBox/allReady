using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class DeleteCampaignCommandHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var campaign = new Campaign { Id = 1 };
            Context.Campaigns.Add(campaign);
            Context.SaveChanges();
        }

        [Fact]
        public async Task DeleteAnExistingCampaignWithMatchingCampaignId()
        {
            var command = new DeleteCampaignCommandAsync { CampaignId = 1 };
            var handler = new DeleteCampaignCommandHandlerAsync(Context);
            await handler.Handle(command);

            var result = Context.Campaigns.Count();
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task NotDeleteAnExistingCampaignWithNonMatchingCampaignId()
        {
            var command = new DeleteCampaignCommandAsync { CampaignId = 2 };
            var handler = new DeleteCampaignCommandHandlerAsync(Context);
            await handler.Handle(command);

            var result = Context.Campaigns.Count();
            Assert.Equal(1, result);
        }
    }
}