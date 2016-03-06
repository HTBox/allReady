using System.Collections.Generic;
using AllReady.Features.Campaigns;
using AllReady.Models;
using AllReady.ViewModels;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Campaigns
{
    public class UnlockedCampaignsQueryHandlerTests
    {
        [Fact]
        public void HandleReturnsOnlyUnLockedCampaigns()
        {
            var campaigns = new List<Campaign>
            {
                new Campaign { Id = 1, Locked = false, ManagingOrganization = new Organization() },
                new Campaign { Id = 2, Locked = true, ManagingOrganization = new Organization() }
            };

            var mockedDataAccess = new Mock<IAllReadyDataAccess>();
            mockedDataAccess.Setup(m => m.Campaigns).Returns(campaigns);

            var sut = new UnlockedCampaignsQueryHandler(mockedDataAccess.Object);
            var results = sut.Handle(new UnlockedCampaignsQuery());

            Assert.Equal(campaigns[0].Id, results[0].Id);
        }

        [Fact]
        public void HandleReturnsCampaignViewModels()
        {
            var sut = new UnlockedCampaignsQueryHandler(new Mock<IAllReadyDataAccess>().Object);
            var results = sut.Handle(new UnlockedCampaignsQuery());

            Assert.IsType<List<CampaignViewModel>>(results);
        }
    }
}
