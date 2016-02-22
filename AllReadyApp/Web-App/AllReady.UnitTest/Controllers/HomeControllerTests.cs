using System;
using AllReady.Controllers;
using AllReady.Models;
using Microsoft.AspNet.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using AllReady.ViewModels;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void IndexGetsCampaignsThatAreNotLocked()
        {
            var campaign = new Campaign { EndDateTime = DateTime.UtcNow.AddDays(1).Date };
            var lockedCampaign = new Campaign { EndDateTime = DateTime.UtcNow.AddDays(1).Date, Locked = true };
            var campaigns = new List<Campaign>
            {
                lockedCampaign,
                campaign
            };

            var mockDataAccess = new Mock<IAllReadyDataAccess>();
            mockDataAccess.Setup(x => x.Campaigns).Returns(campaigns);

            var sut = new HomeController(mockDataAccess.Object);

            var actionResult = (ViewResult)sut.Index();
            var model = (List<CampaignViewModel>)actionResult.ViewData.Model;

            Assert.Equal(campaign.EndDateTime, model.Select(m => m.EndDate).Single());
        }

        [Fact]
        public void IndexGetsCampaignsWithAnEndDateGreaterThanToday()
        {
            var campaignThatEndedYesterday = new Campaign { EndDateTime = DateTime.UtcNow.AddDays(-1).Date };
            var campaignThatEndsTomorrow = new Campaign { EndDateTime = DateTime.UtcNow.AddDays(1).Date };
            var campaigns = new List<Campaign>
            {
                campaignThatEndedYesterday, campaignThatEndsTomorrow
            };

            var mockDataAccess = new Mock<IAllReadyDataAccess>();
            mockDataAccess.Setup(x => x.Campaigns).Returns(campaigns);

            var sut = new HomeController(mockDataAccess.Object);
            var actionResult = (ViewResult)sut.Index();
            var model = (List<CampaignViewModel>)actionResult.ViewData.Model;

            Assert.Equal(campaignThatEndsTomorrow.EndDateTime, model.Select(m => m.EndDate).Single());
        }

        [Fact]
        public void ErrorReturnsTheCorrectView()
        {
            var controller = new HomeController(null);
            var result = (ViewResult) controller.Error();
            Assert.Equal("~/Views/Shared/Error.cshtml", result.ViewName);
        }

        [Fact]
        public void AccessDeniedReturnsTheCorrectView()
        {
            var controller = new HomeController(null);
            var result = (ViewResult)controller.AccessDenied();
            Assert.Equal("~/Views/Shared/AccessDenied.cshtml", result.ViewName);
        }

        [Fact]
        public void AboutShouldReturnAView()
        {
            var sut = new HomeController(null);
            var result = sut.About();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void AesopShouldReturnAView()
        {
            var sut = new HomeController(null);
            var result = sut.Aesop();
            Assert.IsType<ViewResult>(result);
        }
    }
}
