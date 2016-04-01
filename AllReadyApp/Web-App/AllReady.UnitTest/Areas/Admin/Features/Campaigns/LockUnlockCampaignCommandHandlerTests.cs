using AllReady.Areas.Admin.Features.Campaigns;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class LockUnlockCampaignCommandHandlerTests : InMemoryContextTest
    {
        [Fact]
        public async Task LockedCampaignIsUnlocked()
        {
            // Arrange
            var handler = new LockUnlockCampaignCommandHandler(Context);

            // Act
            var campaign = Context.Campaigns.FirstOrDefault(c => c.Name == "Locked Campaign");
            await handler.Handle(new LockUnlockCampaignCommand { CampaignId = campaign.Id });
            var result = Context.Campaigns.FirstOrDefault(c => c.Name == "Locked Campaign");

            // Assert
            Assert.False(result.Locked); // Campaign should now be unlocked
        }

        [Fact]
        public async Task UnlockedCampaignIsLocked()
        {
            // Arrange
            var handler = new LockUnlockCampaignCommandHandler(Context);

            // Act
            var campaign = Context.Campaigns.FirstOrDefault(c => c.Name == "Unlocked Campaign");
            await handler.Handle(new LockUnlockCampaignCommand { CampaignId = campaign.Id });
            var result = Context.Campaigns.FirstOrDefault(c => c.Name == "Unlocked Campaign");

            // Assert
            Assert.True(result.Locked); // Campaign should now be locked
        }

        protected override void LoadTestData()
        {
            CampaignsHandlerTestHelper.LoadCampaignssHandlerTestData(Context);
        }
    }
}