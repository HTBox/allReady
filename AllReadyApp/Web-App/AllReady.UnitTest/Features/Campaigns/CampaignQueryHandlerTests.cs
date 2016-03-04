using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Extensions;
using AllReady.Features.Campaigns;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Campaigns
{
    public class CampaignQueryHandlerTests
    {
        [Fact]
        public void CampaignQueryHandlerReturnsCampaignsThatAreNotLocked()
        {
            var campaign = new Campaign { EndDateTime = DateTime.UtcNow.AddDays(1).Date, ManagingOrganization = new Organization() };
            var lockedCampaign = new Campaign { EndDateTime = DateTime.UtcNow.AddDays(1).Date, Locked = true, ManagingOrganization = new Organization() };
            var campaigns = new List<Campaign> { lockedCampaign, campaign };

            var mockDataAccess = new Mock<IAllReadyDataAccess>();
            mockDataAccess.Setup(x => x.Campaigns).Returns(campaigns);

            var sut = new CampaignQueryHandler(mockDataAccess.Object);
            var model = sut.Handle(new CampaignQuery());

            Assert.Equal(campaign.EndDateTime, model.Select(m => m.EndDate).Single());
        }

        [Fact]
        public void CampaignQueryHandlerReturnsCampaignsWithAnEndDateGreaterThanToday()
        {
            var campaignThatEndedYesterday = new Campaign { EndDateTime = DateTime.UtcNow.AddDays(-1).Date, ManagingOrganization = new Organization() };
            var campaignThatEndsTomorrow = new Campaign { EndDateTime = DateTime.UtcNow.AddDays(1).Date, ManagingOrganization = new Organization() };
            var campaigns = new List<Campaign>
            {
                campaignThatEndedYesterday, campaignThatEndsTomorrow
            };

            var mockDataAccess = new Mock<IAllReadyDataAccess>();
            mockDataAccess.Setup(x => x.Campaigns).Returns(campaigns);

            var sut = new CampaignQueryHandler(mockDataAccess.Object);
            var model = sut.Handle(new CampaignQuery());

            Assert.Equal(campaignThatEndsTomorrow.EndDateTime, model.Select(m => m.EndDate).Single());
        }

        [Fact]
        public void CampaignQueryHandlerReturnsCampaignsOrderedByEndDateAscending()
        {
            var campaigns = new List<Campaign>
            {
                new Campaign { EndDateTime = DateTime.UtcNow.Date, ManagingOrganization = new Organization() }, 
                new Campaign { EndDateTime = DateTime.UtcNow.AddDays(1).Date, ManagingOrganization = new Organization() }
            };

            var mockDataAccess = new Mock<IAllReadyDataAccess>();
            mockDataAccess.Setup(x => x.Campaigns).Returns(campaigns);

            var sut = new CampaignQueryHandler(mockDataAccess.Object);
            var model = sut.Handle(new CampaignQuery());

            Assert.Equal(model.IsOrderedByAscending(x => x.EndDate), true);
        }

        [Fact]
        public void CampaignQueryHandlerReturnsEmptyModelWhenNoResultFromQuery()
        {
            var sut = new CampaignQueryHandler(new Mock<IAllReadyDataAccess>().Object);
            var model = sut.Handle(new CampaignQuery());

            Assert.Equal(model.Count, 0);
        }
    }
}