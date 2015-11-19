using System.Security.Claims;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.Services;
using MediatR;
using Microsoft.AspNet.Http;
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

        [Fact]
        public void CampaignDetailReturnsUnauthorizedForNonAdmin()
        {
            const int tenantId = 1;
            var controller = SetupCampaignController(tenantId);

            var claimsProvider = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.UserType, UserType.BasicUser.ToString()),
                            new Claim(AllReady.Security.ClaimTypes.Tenant, tenantId.ToString()),
                        }));
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User)
                .Returns(() => claimsProvider);
            var mockContext = new Mock<ActionContext>();
            mockContext.Object.HttpContext = mockHttpContext.Object;
            controller.ActionContext = mockContext.Object;

            var actionResult = controller.Details(0);
            Assert.IsType<HttpUnauthorizedResult>(actionResult);
        }


        [Fact]
        public void CampaignDetailReturnsViewForAdmin()
        {
            const int tenantId = 1;
            var controller = SetupCampaignController(tenantId);

            var claimsProvider = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.UserType, UserType.TenantAdmin.ToString()),
                            new Claim(AllReady.Security.ClaimTypes.Tenant, tenantId.ToString()),
                        }));
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User)
                .Returns(() => claimsProvider);
            var mockContext = new Mock<ActionContext>();
            mockContext.Object.HttpContext = mockHttpContext.Object;
            controller.ActionContext = mockContext.Object;

            var actionResult = controller.Details(0);
            Assert.IsType<ViewResult>(actionResult);
        }

        private static CampaignController SetupCampaignController(int tenantId)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignDetailQuery>()))
                .Returns(() => new CampaignDetailModel {TenantId = tenantId})
                .Verifiable();
            var mockImageService = new Mock<IImageService>();
            var mockDataAccess = new Mock<IAllReadyDataAccess>();
            var controller = new CampaignController(
                mockMediator.Object,
                mockImageService.Object,
                mockDataAccess.Object);
            return controller;
        }
    }
}
