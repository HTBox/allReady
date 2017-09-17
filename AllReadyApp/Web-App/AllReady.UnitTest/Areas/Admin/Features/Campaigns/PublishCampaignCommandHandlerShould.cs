using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class PublishCampaignCommandHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var campaign = new Campaign { Id = 1 };
            Context.Campaigns.Add(campaign);
            Context.SaveChanges();
        }

        [Fact]
        public async Task ChangePublishedPropertyToTrue()
        {
            var command = new PublishCampaignCommand { CampaignId = 1 };
            var handler = new PublishCampaignCommandHandler(Context);
            await handler.Handle(command);

            var result = Context.Campaigns.Single(c => c.Id == 1);

            Assert.True(result.Published);
        }
    }
}
