using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
            const int organizationId = 1, campaignId = 0;
            var controller = CampaignControllerWithDetailQuery(UserType.BasicUser.ToString(), organizationId);
            
            Assert.IsType<HttpUnauthorizedResult>(controller.Details(campaignId));
        }

        [Fact]
        public void CampaignEditReturnsUnauthorizedForNonAdmin()
        {
            const int organizationId = 1, campaignId = 0;
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), organizationId);
            
            Assert.IsType<HttpUnauthorizedResult>(controller.Edit(campaignId));
        }

        [Fact]
        public void CampaignDeleteReturnsUnauthorizedForNonAdmin()
        {
            const int organizationId = 1, campaignId = 0;
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), organizationId);
            
            Assert.IsType<HttpUnauthorizedResult>(controller.Delete(campaignId));
        }
        #endregion

        #region Test returing ViewResult for Organization Admin on a campaign
        [Fact]
        public void CampaignDetailReturnsViewForAdmin()
        {
            const int organizationId = 1, campaignId = 0;
            var controller = CampaignControllerWithDetailQuery(UserType.OrgAdmin.ToString(), organizationId);
            
            Assert.IsType<ViewResult>(controller.Details(campaignId));
        }

        [Fact]
        public void CampaignEditReturnsViewForAdmin()
        {
            const int organizationId = 1, campaignId = 0;
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), organizationId);
            
            Assert.IsType<ViewResult>(controller.Edit(campaignId));
        }

        [Fact]
        public void CampaignDeleteReturnsViewForAdmin()
        {
            const int organizationId = 1, campaignId = 0;
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), organizationId);
            
            Assert.IsType<ViewResult>(controller.Delete(campaignId));
        }
        #endregion

        #region Edit Post
        [Fact]
        public void CampaignEditPostReturnsBadRequestForNullCampaign()
        {
            const int organizationId = 1;
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), organizationId);

            var result = controller.Edit(null, null).Result as BadRequestResult;
            Assert.NotNull(result);
        }

        [Fact]
        public void CampaignEditPostReturnsUnAuthorizedForNullCampaign()
        {
            const int organizationId = 1;
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), organizationId);

            Assert.IsType<HttpUnauthorizedResult>(
                controller.Edit(new CampaignSummaryModel { OrganizationId = organizationId }, null).Result);
        }

        [Fact]
        public void CampaignEditPostReturnsUnAuthorizedForBasicUser()
        {
            const int organizationId = 1;
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), organizationId);

            Assert.IsType<HttpUnauthorizedResult>(
                controller.Edit(new CampaignSummaryModel { OrganizationId = organizationId }, null).Result);
        }

        [Fact]
        public void CampaignEditPostReturnsViewResultForInvalidModel()
        {
            const int organizationId = 1;
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), organizationId);
            // Force an invalid model
            controller.ModelState.AddModelError("foo","bar");

            Assert.IsType<ViewResult>(
                controller.Edit(new CampaignSummaryModel { OrganizationId = organizationId }, null).Result);
        }

        [Fact]
        public void CampaignEditPostReturnsRedirectToActionResultForValidModel()
        {
            const int organizationId = 1;
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), organizationId);

            Assert.IsType<RedirectToActionResult>(
                controller.Edit(new CampaignSummaryModel { Name = "Foo", OrganizationId = organizationId }, null).Result);
        }

        [Fact]
        public void CampaignEditPostHasModelErrorWhenInvalidImageFormatIsSupplied()
        {
            const int organizationId = 1;
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), organizationId);
            var file = FormFile("");

            var result = controller.Edit(new CampaignSummaryModel { Name = "Foo", OrganizationId = organizationId }, file).GetAwaiter().GetResult();
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("ImageUrl"));
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task CampaignEditPostUploadsImageToImageService()
        {
            const int organizationId = 1;
            const int campaignId = 100;
            var mockMediator = new Mock<IMediator>();
            var mockImageService = new Mock<IImageService>();
            mockImageService
                .Setup(mock => mock.UploadCampaignImageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IFormFile>()))
                .Returns(() => Task.FromResult(""))
                .Verifiable();
            var controller = new CampaignController(
                mockMediator.Object,
                mockImageService.Object);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User)
                .Returns(() => GetClaimsPrincipal(UserType.OrgAdmin.ToString(), organizationId));
            var mockContext = new Mock<ActionContext>();
            mockContext.Object.HttpContext = mockHttpContext.Object;
            controller.ActionContext = mockContext.Object;

            var file = FormFile("image/jpeg");

            await controller.Edit(new CampaignSummaryModel
            {
                Name = "Foo",
                OrganizationId = organizationId,
                Id = campaignId
            }, file).ConfigureAwait(false);
            mockImageService.Verify(mock => 
                mock.UploadCampaignImageAsync(
                        It.Is<int>(i => i == organizationId), 
                        It.Is<int>(i => i == campaignId), 
                        It.Is<IFormFile>(i => i == file)), 
                Times.Once);
        }
        #endregion

        #region Delete Contirmed Post

        [Fact]
        public void CampaignDetailConfirmedReturnsUnauthorizedForNonAdmin()
        {
            const int organizationId = 1;
            const int campaignId = 100;
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), organizationId);

            Assert.IsType<HttpUnauthorizedResult>(controller.DeleteConfirmed(campaignId));
        }

        [Fact]
        public void CampaignDetailConfirmedMockChecksForAdminUser()
        {
            const int organizationId = 1;
            const int campaignId = 100;
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignSummaryQuery>()))
                .Returns(() => new CampaignSummaryModel { OrganizationId = organizationId })
                .Verifiable();
            mockMediator.Setup(mock => mock.Send(It.IsAny<DeleteCampaignCommand>()))
                .Verifiable();
            var mockImageService = new Mock<IImageService>();
            var controller = new CampaignController(
                mockMediator.Object,
                mockImageService.Object);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User)
                .Returns(() => GetClaimsPrincipal(UserType.OrgAdmin.ToString(), organizationId));
            var mockContext = new Mock<ActionContext>();
            mockContext.Object.HttpContext = mockHttpContext.Object;
            controller.ActionContext = mockContext.Object;

            var result = controller.DeleteConfirmed(campaignId);
            Assert.IsType<RedirectToActionResult>(result);
            mockMediator.Verify(mock => mock.Send(It.Is<CampaignSummaryQuery>(i => i.CampaignId == campaignId)), Times.Once);
            mockMediator.Verify(mock => mock.Send(It.Is<DeleteCampaignCommand>(i => i.CampaignId == campaignId)), Times.Once);
        }
        #endregion

        #region Helper Methods
        private static Mock<IMediator> MockMediatorCampaignDetailQuery(out CampaignController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignDetailQuery>())).Returns(() => null).Verifiable();
            var mockImageService = new Mock<IImageService>();
            controller = new CampaignController(
                mockMediator.Object,
                mockImageService.Object);
            return mockMediator;
        }

        private static Mock<IMediator> MockMediatorCampaignSummaryQuery(out CampaignController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignSummaryQuery>())).Returns(() => null).Verifiable();
            var mockImageService = new Mock<IImageService>();
            controller = new CampaignController(
                mockMediator.Object,
                mockImageService.Object);
            return mockMediator;
        }

        private static CampaignController CampaignControllerWithDetailQuery(string userType, int organizationId)
        {
            var tid = organizationId;
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignDetailQuery>()))
                .Returns(() => new CampaignDetailModel { OrganizationId = tid })
                .Verifiable();
            var mockImageService = new Mock<IImageService>();
            var controller = new CampaignController(
                mockMediator.Object,
                mockImageService.Object);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User)
                .Returns(() => GetClaimsPrincipal(userType, organizationId));
            var mockContext = new Mock<ActionContext>();
            mockContext.Object.HttpContext = mockHttpContext.Object;
            controller.ActionContext = mockContext.Object;
            return controller;
        }

        private static CampaignController CampaignControllerWithSummaryQuery(string userType, int organizationId)
        {
            var tid = organizationId;
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignSummaryQuery>()))
                .Returns(() => new CampaignSummaryModel { OrganizationId = tid, Location = new LocationEditModel() })
                .Verifiable();
            var mockImageService = new Mock<IImageService>();
            var controller = new CampaignController(
                mockMediator.Object,
                mockImageService.Object);
            
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User)
                .Returns(() => GetClaimsPrincipal(userType, organizationId));
            var mockContext = new Mock<ActionContext>();
            mockContext.Object.HttpContext = mockHttpContext.Object;
            controller.ActionContext = mockContext.Object;
            return controller;
        }

        private static ClaimsPrincipal GetClaimsPrincipal(string userType, int organizationId)
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(AllReady.Security.ClaimTypes.UserType, userType),
                        new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString()),
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
