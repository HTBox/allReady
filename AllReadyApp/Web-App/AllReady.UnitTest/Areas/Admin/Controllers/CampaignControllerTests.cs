using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using AllReady.Services;
using MediatR;
using Moq;
using Xunit;
using Microsoft.AspNet.Mvc;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class CampaignControllerTests
    {

        [Fact]
        public void CampaignDetailsNoCampaignReturns404()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignDetailQuery>())).Returns(() => null).Verifiable();
            var mockImageService = new Mock<IImageService>();
            var mockDataAccess = new Mock<IAllReadyDataAccess>();
            var controller = new CampaignController(
                mockMediator.Object,
                mockImageService.Object,
                mockDataAccess.Object );
            var actionResult = controller.Details(0);
            Assert.IsType<HttpNotFoundResult>(actionResult);
            mockMediator.Verify(mock => mock.Send(It.IsAny<CampaignDetailQuery>()), Times.Once);
        }
    }
}
