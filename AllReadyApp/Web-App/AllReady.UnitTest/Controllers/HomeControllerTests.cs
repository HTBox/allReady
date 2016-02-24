using AllReady.Controllers;
using AllReady.Features.Campaigns;
using MediatR;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void IndexSendsCampaignQuery()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.Send(It.IsAny<CampaignQuery>())).Returns(new CampaignModel());

            var sut = new HomeController(mockMediator.Object);
            var result = sut.Index();

            mockMediator.Verify(x => x.Send(It.IsAny<CampaignQuery>()), Times.Once());
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
    }
}
