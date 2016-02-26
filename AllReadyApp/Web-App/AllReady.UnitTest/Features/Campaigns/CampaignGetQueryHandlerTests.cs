using System.Collections.Generic;
using AllReady.Features.Campaigns;
using AllReady.Models;
using AllReady.ViewModels;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Campaigns
{
    public class CampaignGetQueryHandlerTests
    {
        [Fact]
        public void HandleReturnsOnlyUnLockedCampaigns()
        {
            var campaigns = new List<Campaign>
            {
                new Campaign { Id = 1, Locked = false },
                new Campaign { Id = 2, Locked = true }
            };

            var mockedDataAccess = new Mock<IAllReadyDataAccess>();
            mockedDataAccess.Setup(m => m.Campaigns).Returns(campaigns);

            var sut = new CampaignGetQueryHandler(mockedDataAccess.Object);
            var results = sut.Handle(new CampaignGetQuery());

            Assert.Equal(campaigns[0].Id, results[0].Id);
        }

        [Fact]
        public void HandleReturnsCampaignViewModels()
        {
            var sut = new CampaignGetQueryHandler(new Mock<IAllReadyDataAccess>().Object);
            var results = sut.Handle(new CampaignGetQuery());

            Assert.IsType<List<CampaignViewModel>>(results);
        }
    }
}
