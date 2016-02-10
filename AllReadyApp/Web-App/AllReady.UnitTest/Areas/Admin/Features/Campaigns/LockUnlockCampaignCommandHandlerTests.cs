using AllReady.Areas.Admin.Features.Campaigns;
using System.Linq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class LockUnlockCampaignCommandHandlerTests : InMemoryContextTest
    {
        [Fact]
        public void LockedCampaignIsUnlocked()
        {
            // Arrange
            var handler = new LockUnlockCampaignCommandHandler(Context);

            // Act
            var campaign = Context.Campaigns.FirstOrDefault(c => c.Name == "Locked Campaign");
            handler.Handle(new LockUnlockCampaignCommand { CampaignId = campaign.Id });
            var result = Context.Campaigns.FirstOrDefault(c => c.Name == "Locked Campaign");

            // Assert
            Assert.False(result.Locked); // Campaign should now be unlocked
        }

        [Fact]
        public void UnlockedCampaignIsLocked()
        {
            // Arrange
            var handler = new LockUnlockCampaignCommandHandler(Context);

            // Act
            var campaign = Context.Campaigns.FirstOrDefault(c => c.Name == "Unlocked Campaign");
            handler.Handle(new LockUnlockCampaignCommand { CampaignId = campaign.Id });
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