using AllReady.Features.Campaigns;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Campaigns
{
    public class CampaginByCampaignIdQueryHandlerTests
    {
        [Fact]
        public void HandleCallsGetCampaignWithCorrectCampaignId()
        {
            var message = new CampaignByCampaignIdQuery { CampaignId = 1 };
            var mockedDataAccess = new Mock<IAllReadyDataAccess>();

            var sut = new CampaignByCampaignIdQueryHandler(mockedDataAccess.Object);
            sut.Handle(message);

            mockedDataAccess.Verify(x => x.GetCampaign(message.CampaignId));
        }

        [Fact]
        public void HandleReturnsCorrectCampaign()
        {
            var message = new CampaignByCampaignIdQuery { CampaignId = 1 };
            var campaign = new Campaign { Id = message.CampaignId };

            var mockedDataAccess = new Mock<IAllReadyDataAccess>();
            mockedDataAccess.Setup(m => m.GetCampaign(message.CampaignId)).Returns(campaign);

            var sut = new CampaignByCampaignIdQueryHandler(mockedDataAccess.Object);
            var result = sut.Handle(message);

            Assert.Same(campaign, result);
        }
    }
}
