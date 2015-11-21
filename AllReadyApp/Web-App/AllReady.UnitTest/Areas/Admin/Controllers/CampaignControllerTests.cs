using System;
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
        #region Test returning 404 for campaign not found
        [Fact]
        public void CampaignDetailsNoCampaignReturns404()
        {
            const int campaignId = 0;
            CampaignController controller;
            var mockMediator = MockMediatorCampaignDetailQuery(out controller);
            
            Assert.IsType<HttpNotFoundResult>(controller.Details(campaignId));
            mockMediator.Verify(mock => mock.Send(It.IsAny<CampaignDetailQuery>()), Times.Once);
        }

        [Fact]
        public void CampaignEditNoCampaignReturns404()
        {
            const int campaignId = 0;
            CampaignController controller;
            var mockMediator = MockMediatorCampaignSummaryQuery(out controller);
            
            Assert.IsType<HttpNotFoundResult>(controller.Edit(campaignId));
            mockMediator.Verify(mock => mock.Send(It.IsAny<CampaignSummaryQuery>()), Times.Once);
        }

        [Fact]
        public void CampaignDeleteNoCampaignReturns404()
        {
            const int campaignId = 0;
            CampaignController controller;
            var mockMediator = MockMediatorCampaignSummaryQuery(out controller);
            
            Assert.IsType<HttpNotFoundResult>(controller.Delete(campaignId));
            mockMediator.Verify(mock => mock.Send(It.IsAny<CampaignSummaryQuery>()), Times.Once);
        }

        #endregion


        #region Test returing 401 for unauthorized user on a campaign
        [Fact]
        public void CampaignDetailReturnsUnauthorizedForNonAdmin()
        {
            const int tenantId = 1, campaignId = 0;
            var controller = CampaignControllerWithDetailQuery(UserType.BasicUser.ToString(), tenantId);
            
            Assert.IsType<HttpUnauthorizedResult>(controller.Details(campaignId));
        }

        [Fact]
        public void CampaignEditReturnsUnauthorizedForNonAdmin()
        {
            const int tenantId = 1, campaignId = 0;
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), tenantId);
            
            Assert.IsType<HttpUnauthorizedResult>(controller.Edit(campaignId));
        }

        [Fact]
        public void CampaignDeleteReturnsUnauthorizedForNonAdmin()
        {
            const int tenantId = 1, campaignId = 0;
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), tenantId);
            
            Assert.IsType<HttpUnauthorizedResult>(controller.Delete(campaignId));
        }
        #endregion



        #region Test returing ViewResult for Tenant Admin on a campaign
        [Fact]
        public void CampaignDetailReturnsViewForAdmin()
        {
            const int tenantId = 1, campaignId = 0;
            var controller = CampaignControllerWithDetailQuery(UserType.TenantAdmin.ToString(), tenantId);
            
            Assert.IsType<ViewResult>(controller.Details(campaignId));
        }

        [Fact]
        public void CampaignEditReturnsViewForAdmin()
        {
            const int tenantId = 1, campaignId = 0;
            var controller = CampaignControllerWithSummaryQuery(UserType.TenantAdmin.ToString(), tenantId);
            
            Assert.IsType<ViewResult>(controller.Edit(campaignId));
        }

        [Fact]
        public void CampaignDeleteReturnsViewForAdmin()
        {
            const int tenantId = 1, campaignId = 0;
            var controller = CampaignControllerWithSummaryQuery(UserType.TenantAdmin.ToString(), tenantId);
            
            Assert.IsType<ViewResult>(controller.Delete(campaignId));
        }
        #endregion

        #region Edit Post
        [Fact]
        public void CampaignEditPostReturnsBadRequestForNullCampaign()
        {
            const int tenantId = 1;
            var controller = CampaignControllerWithSummaryQuery(UserType.TenantAdmin.ToString(), tenantId);

            var result = controller.Edit(null, null).Result as BadRequestResult;
            Assert.NotNull(result);
        }

        [Fact]
        public void CampaignEditPostReturnsUnAuthorizedForNullCampaign()
        {
            const int tenantId = 1;
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), tenantId);

            Assert.IsType<HttpUnauthorizedResult>(
                controller.Edit(new CampaignSummaryModel { TenantId = tenantId }, null).Result);
        }

        [Fact]
        public void CampaignEditPostReturnsUnAuthorizedForBasicUser()
        {
            const int tenantId = 1;
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), tenantId);

            Assert.IsType<HttpUnauthorizedResult>(
                controller.Edit(new CampaignSummaryModel { TenantId = tenantId }, null).Result);
        }

        [Fact]
        public void CampaignEditPostReturnsViewResultForInvalidModel()
        {
            const int tenantId = 1;
            var controller = CampaignControllerWithSummaryQuery(UserType.TenantAdmin.ToString(), tenantId);
            // Force an invalid model
            controller.ModelState.AddModelError("foo","bar");

            Assert.IsType<ViewResult>(
                controller.Edit(new CampaignSummaryModel { TenantId = tenantId }, null).Result);
        }

        [Fact]
        public void CampaignEditPostReturnsRedirectToActionResultForValidModel()
        {
            const int tenantId = 1;
            var controller = CampaignControllerWithSummaryQuery(UserType.TenantAdmin.ToString(), tenantId);

            Assert.IsType<RedirectToActionResult>(
                controller.Edit(new CampaignSummaryModel { Name = "Foo", TenantId = tenantId }, null).Result);
        }

        [Fact]
        public void CampaignEditPostHasModelErrorWhenInvalidImageFormatIsSupplied()
        {
            const int tenantId = 1;
            var controller = CampaignControllerWithSummaryQuery(UserType.TenantAdmin.ToString(), tenantId);
            var file = FormFile("");

            var result = controller.Edit(new CampaignSummaryModel { Name = "Foo", TenantId = tenantId }, file).Result;
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("ImageUrl"));
            Assert.IsType<ViewResult>(result);
        }

        // ToDo: Add test for Image Service calls 
        #endregion

        #region Helper Methods
        private static Mock<IMediator> MockMediatorCampaignDetailQuery(out CampaignController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignDetailQuery>())).Returns(() => null).Verifiable();
            var mockImageService = new Mock<IImageService>();
            var mockDataAccess = new Mock<IAllReadyDataAccess>();
            controller = new CampaignController(
                mockMediator.Object,
                mockImageService.Object,
                mockDataAccess.Object);
            return mockMediator;
        }

        private static Mock<IMediator> MockMediatorCampaignSummaryQuery(out CampaignController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignSummaryQuery>())).Returns(() => null).Verifiable();
            var mockImageService = new Mock<IImageService>();
            var mockDataAccess = new Mock<IAllReadyDataAccess>();
            controller = new CampaignController(
                mockMediator.Object,
                mockImageService.Object,
                mockDataAccess.Object);
            return mockMediator;
        }

        private static CampaignController CampaignControllerWithDetailQuery(string userType, int tenantId)
        {
            var tid = tenantId;
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignDetailQuery>()))
                .Returns(() => new CampaignDetailModel { TenantId = tid })
                .Verifiable();
            var mockImageService = new Mock<IImageService>();
            var mockDataAccess = new Mock<IAllReadyDataAccess>();
            var controller = new CampaignController(
                mockMediator.Object,
                mockImageService.Object,
                mockDataAccess.Object);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User)
                .Returns(() => GetClaimsPrincipal(userType, tenantId));
            var mockContext = new Mock<ActionContext>();
            mockContext.Object.HttpContext = mockHttpContext.Object;
            controller.ActionContext = mockContext.Object;
            return controller;
        }

        private static CampaignController CampaignControllerWithSummaryQuery(string userType, int tenantId)
        {
            var tid = tenantId;
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignSummaryQuery>()))
                .Returns(() => new CampaignSummaryModel { TenantId = tid })
                .Verifiable();
            var mockImageService = new Mock<IImageService>();
            var mockDataAccess = new Mock<IAllReadyDataAccess>();
            var controller = new CampaignController(
                mockMediator.Object,
                mockImageService.Object,
                mockDataAccess.Object);
            
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User)
                .Returns(() => GetClaimsPrincipal(userType, tenantId));
            var mockContext = new Mock<ActionContext>();
            mockContext.Object.HttpContext = mockHttpContext.Object;
            controller.ActionContext = mockContext.Object;
            return controller;
        }

        private static ClaimsPrincipal GetClaimsPrincipal(string userType, int tenantId)
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(AllReady.Security.ClaimTypes.UserType, userType),
                        new Claim(AllReady.Security.ClaimTypes.Tenant, tenantId.ToString()),
                    }));
        }

        private static IFormFile FormFile(string fileType)
        {
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(mock => mock.ContentType).Returns(fileType);
            return mockFormFile.Object;
        }
        #endregion
    }
}
