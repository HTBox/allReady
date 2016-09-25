using AllReady.Controllers;
using AllReady.Features.Campaigns;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using AllReady.Features.Home;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public async Task IndexSendsActiveOrUpcomingCampaignsQuery()
        {
            var mockMediator = new Mock<IMediator>();

            var sut = new HomeController(mockMediator.Object);
            await sut.Index();

            mockMediator.Verify(x => x.SendAsync(It.IsAny<ActiveOrUpcomingCampaignsQuery>()), Times.Once());
        }

        [Fact]
        public async Task IndexSendsFeaturedCampaignQuery()
        {
            var mockMediator = new Mock<IMediator>();

            var sut = new HomeController(mockMediator.Object);
            await sut.Index();

            mockMediator.Verify(x => x.SendAsync(It.IsAny<FeaturedCampaignQuery>()), Times.Once());
        }

        [Fact]
        public async Task IndexReturnsCorrectViewResult()
        {
            var mockMediator = new Mock<IMediator>();

            var sut = new HomeController(mockMediator.Object);
            var result = (ViewResult)await sut.Index();

            Assert.NotNull(result);
        }

        [Fact]
        public void ErrorReturnsTheCorrectView()
        {
            var controller = new HomeController(null);
            var result = (ViewResult)controller.Error();
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
        public void AboutReturnsAView()
        {
            var sut = new HomeController(null);
            var result = sut.About();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void AesopReturnsAView()
        {
            var sut = new HomeController(null);
            var result = sut.Aesop();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void PrivacyPolicy_ReturnsCorrectView()
        {
            var controller = new HomeController(Mock.Of<IMediator>());

            var result = (ViewResult)controller.PrivacyPolicy();

            Assert.NotNull(result);
            Assert.Equal("PrivacyPolicy", result.ViewName);
        }
    }
}