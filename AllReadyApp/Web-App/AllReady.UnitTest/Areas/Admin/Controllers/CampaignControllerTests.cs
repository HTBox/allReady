using System;
using System.Collections.Generic;
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
        //delete this line when all unit tests using it have been completed
        private readonly Task taskFromResultZero = Task.FromResult(0);

        [Fact(Skip = "NotImplemented")]
        public void IndexSendsCampaignListQueryWithCorrectDataWhenUserIsOrgAdmin()
        {
        }
            
        [Fact(Skip = "NotImplemented")]
        public void IndexSendsCampaignListQueryWithCorrectDataWhenUserIsNotOrgAdmin()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void IndexReturnsCorrectViewModel()
        {
        }
            
        [Fact(Skip = "NotImplemented")]
        public async Task CampaignDetailsSendsCampaignDetailQueryWithCorrectData()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundResultWhenVieModelIsNull()
        {
            CampaignController controller;
            MockMediatorCampaignDetailQuery(out controller);
            Assert.IsType<HttpNotFoundResult>(await controller.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsHttpUnauthorizedResultIfUserIsNotOrgAdmin()
        {
            var controller = CampaignControllerWithDetailQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            Assert.IsType<HttpUnauthorizedResult>(await controller.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsCorrectViewWhenViewModelIsNotNullAndUserIsOrgAdmin()
        {
            var controller = CampaignControllerWithDetailQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            Assert.IsType<ViewResult>(await controller.Details(It.IsAny<int>()));
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DetailsReturnsCorrectViewModelWhenViewModelIsNotNullAndUserIsOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void CreateReturnsCorrectViewWithCorrectViewModel()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditGetSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
        }

        [Fact]
        public void EditGetReturnsHttpNotFoundResultWhenViewModelIsNull()
        {
            CampaignController controller;
            MockMediatorCampaignSummaryQuery(out controller);
            Assert.IsType<HttpNotFoundResult>(controller.Edit(It.IsAny<int>()));
        }

        [Fact]
        public void EditGetReturnsHttpUnauthorizedResultWhenUserIsNotAnOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            Assert.IsType<HttpUnauthorizedResult>(controller.Edit(It.IsAny<int>()));
        }

        [Fact]
        public void EditGetReturnsCorrectViewModelWhenUserIsOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            Assert.IsType<ViewResult>(controller.Edit(It.IsAny<int>()));
        }

        [Fact]
        public async Task EditPostReturnsBadRequestWhenCampaignIsNull()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            var result = await controller.Edit(null, null);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task EditPostReturnsHttpUnauthorizedResultWhenUserIsNotAnOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            var result = await controller.Edit(new CampaignSummaryModel { OrganizationId = It.IsAny<int>() }, null);
            Assert.IsType<HttpUnauthorizedResult>(result);
        }

        [Fact]
        public async Task EditPostRedirectsToCorrectActionWithCorrectRouteValuesWhenModelStateIsValid()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            var result = await controller.Edit(new CampaignSummaryModel { Name = "Foo", OrganizationId = It.IsAny<int>() }, null);

            //TODO: test result for correct Action name and Route values
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task EditPostAddsErrorToModelStateWhenInvalidImageFormatIsSupplied()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            var file = FormFile("");

            await controller.Edit(new CampaignSummaryModel { Name = "Foo", OrganizationId = It.IsAny<int>() }, file);

            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("ImageUrl"));
            //TODO: test that the value associated with the key is correct
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostReturnsCorrectViewModelWhenInvalidImageFormatIsSupplied()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact]
        public async Task EditPostUploadsImageToImageService()
        {
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            var mockImageService = new Mock<IImageService>();
            mockImageService.Setup(mock => mock.UploadCampaignImageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IFormFile>())).Returns(() => Task.FromResult("")).Verifiable();

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User)
                .Returns(() => UnitTestHelper.GetClaimsPrincipal(UserType.OrgAdmin.ToString(), organizationId));
            var mockContext = new Mock<ActionContext>();
            mockContext.Object.HttpContext = mockHttpContext.Object;

            var controller = new CampaignController(mockMediator.Object, mockImageService.Object) { ActionContext = mockContext.Object };

            var file = FormFile("image/jpeg");

            await controller.Edit(new CampaignSummaryModel { Name = "Foo", OrganizationId = organizationId, Id = campaignId}, file);

            mockImageService.Verify(mock => mock.UploadCampaignImageAsync(
                        It.Is<int>(i => i == organizationId), 
                        It.Is<int>(i => i == campaignId), 
                It.Is<IFormFile>(i => i == file)), Times.Once);
        }

        [Fact(Skip = "NotImplemented")]
        public void EditPostHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditPostHasHValidateAntiForgeryTokenttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
        }

        [Fact]
        public void DeleteReturnsHttpNotFoundResultWhenCampaignIsNotFound()
        {
            CampaignController controller;
            MockMediatorCampaignSummaryQuery(out controller);
            Assert.IsType<HttpNotFoundResult>(controller.Delete(It.IsAny<int>()));
        }

        [Fact]
        public void DeleteReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());   
            Assert.IsType<HttpUnauthorizedResult>(controller.Delete(It.IsAny<int>()));
        }

        [Fact]
        public void DeleteReturnsCorrectViewWhenUserIsOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            Assert.IsType<ViewResult>(controller.Delete(It.IsAny<int>()));
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteReturnsCorrectViewModelWhenUserIsOrgAdmin()
        {
        }

        public void DeleteConfirmedSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
            const int campaignId = 1;

            var mediator = new Mock<IMediator>();
            var sut = new CampaignController(mediator.Object, null);
            sut.DeleteConfirmed(campaignId);

            mediator.Verify(mock => mock.Send(It.Is<CampaignSummaryQuery>(i => i.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public void DetailConfirmedReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            Assert.IsType<HttpUnauthorizedResult>(controller.DeleteConfirmed(It.IsAny<int>()));
        }

        [Fact]
        public void DetailConfirmedSendsDeleteCampaignCommandWithCorrectCampaignIdWhenUserIsOrgAdmin()
        {
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignSummaryQuery>()))
                .Returns(() => new CampaignSummaryModel { OrganizationId = organizationId });

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User).Returns(() => GetClaimsPrincipal(UserType.OrgAdmin.ToString(), organizationId));

            var mockContext = new Mock<ActionContext>();
            mockContext.Object.HttpContext = mockHttpContext.Object;

            var controller = new CampaignController(mockMediator.Object, null);
            controller.ActionContext = mockContext.Object;

            controller.DeleteConfirmed(campaignId);

            mockMediator.Verify(mock => mock.Send(It.Is<DeleteCampaignCommand>(i => i.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public void DetailConfirmedRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsOrgAdmin()
        {
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignSummaryQuery>())).Returns(() => new CampaignSummaryModel { OrganizationId = organizationId });

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User)
                .Returns(() => UnitTestHelper.GetClaimsPrincipal(UserType.OrgAdmin.ToString(), organizationId));
            var mockContext = new Mock<ActionContext>();
            mockContext.Object.HttpContext = mockHttpContext.Object;
            
            var controller = new CampaignController(mockMediator.Object, null);
            controller.ActionContext = mockContext.Object;

            var routeValues = new Dictionary<string, object> { ["area"] = "Admin" };

            var result = controller.DeleteConfirmed(campaignId) as RedirectToActionResult;
            Assert.Equal(result.ActionName, nameof(CampaignController.Index));
            Assert.Equal(result.RouteValues, routeValues);
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteConfirmedHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteConfirmedHasActionNameAttributeWithCorrectName()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteConfirmedHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void LockUnlockReturnsHttpUnauthorizedResultWhenUserIsNotSiteAdmin()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void LockUnlockSendsLockUnlockCampaignCommandWithCorrectCampaignIdWhenUserIsSiteAdmin()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void LockUnlockRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsSiteAdmin()
        {
        }

        #region Helper Methods
        private static Mock<IMediator> MockMediatorCampaignDetailQuery(out CampaignController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignDetailQuery>())).Returns(() => null).Verifiable();

            var mockImageService = new Mock<IImageService>();

            controller = new CampaignController(mockMediator.Object, mockImageService.Object);

            return mockMediator;
        }

        private static Mock<IMediator> MockMediatorCampaignSummaryQuery(out CampaignController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignSummaryQuery>())).Returns(() => null).Verifiable();

            var mockImageService = new Mock<IImageService>();
            controller = new CampaignController(mockMediator.Object, mockImageService.Object);
            return mockMediator;
        }

        private static CampaignController CampaignControllerWithDetailQuery(string userType, int organizationId)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignDetailQuery>())).Returns(() => new CampaignDetailModel { OrganizationId = organizationId }).Verifiable();

            var mockImageService = new Mock<IImageService>();

            var controller = new CampaignController(mockMediator.Object, mockImageService.Object);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User)
                .Returns(() => UnitTestHelper.GetClaimsPrincipal(userType, organizationId));
            var mockContext = new Mock<ActionContext>();
            mockContext.Object.HttpContext = mockHttpContext.Object;
            controller.ActionContext = mockContext.Object;

            return controller;
        }

        private static CampaignController CampaignControllerWithSummaryQuery(string userType, int organizationId)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignSummaryQuery>()))
                .Returns(() => new CampaignSummaryModel { OrganizationId = organizationId, Location = new LocationEditModel() }).Verifiable();

            var mockImageService = new Mock<IImageService>();

            var controller = new CampaignController(mockMediator.Object, mockImageService.Object);
            
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User)
                .Returns(() => UnitTestHelper.GetClaimsPrincipal(userType, organizationId));
            var mockContext = new Mock<ActionContext>();
            mockContext.Object.HttpContext = mockHttpContext.Object;
            controller.ActionContext = mockContext.Object;

            return controller;
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
