using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.Services;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNet.Http;
using Moq;
using Xunit;
using Microsoft.AspNet.Mvc;
using System.Linq;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class CampaignAdminControllerTests
    {
        //delete this line when all unit tests using it have been completed
        private readonly Task taskFromResultZero = Task.FromResult(0);

        [Fact]
        public void IndexSendsCampaignListQueryAndReturnsWithCorrectDataWhenUserIsOrgAdmin()
        {
            int organizationId = 100;
            var campaignSummaries = new List<CampaignSummaryModel>
            {
                new CampaignSummaryModel
                {
                    Id = 1,
                    Name = "Summary Model 1",
                    OrganizationId = organizationId
                },
                new CampaignSummaryModel
                {
                    Id = 2,
                    Name = "Summary Model 2",
                    OrganizationId = 1003
                }

            };
            var sut = CampaignControllerWithCampaignListQuery(UserType.OrgAdmin.ToString(), organizationId, campaignSummaries);
            var result = sut.Index();

            var view = ValidateResult<ViewResult>(result);
            var dataList = ValidateResult<List<CampaignSummaryModel>>(view.ViewData.Model);

            Assert.Equal(dataList.Count, campaignSummaries.Count(cs => cs.OrganizationId == organizationId));
        }
            
        [Fact]
        public void IndexSendsCampaignListQueryAndReturnsWithCorrectDataWhenUserIsNotOrgAdmin()
        {
            var organizationId = 100;
            var campaignSummaries = new List<CampaignSummaryModel>
            {
                new CampaignSummaryModel
                {
                    Id = 1,
                    Name = "Summary Model 1",
                    OrganizationId = organizationId
                },
                new CampaignSummaryModel
                {
                    Id = 2,
                    Name = "Summary Model 2",
                    OrganizationId = 1003
                }

            };
            var sut = CampaignControllerWithCampaignListQuery(UserType.BasicUser.ToString(), organizationId, campaignSummaries);
            var result = sut.Index();

            var view = ValidateResult<ViewResult>(result);
            var dataList = ValidateResult<List<CampaignSummaryModel>>(view.ViewData.Model);
            Assert.Equal(dataList.Count, campaignSummaries.Count());
        }

        [Fact]
        public async Task DetailsSendsCampaignDetailQueryWithCorrectCampaignId()
        {
            var mediatorMock = new Mock<IMediator>();
            var sut = new CampaignController(mediatorMock.Object, null);
            var id = 2;
            var result = await sut.Details(id);
            mediatorMock.Verify(mock => mock.SendAsync(It.Is<CampaignDetailQuery>(cq => cq.CampaignId == id)), Times.Once);
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

        [Fact]
        public async Task DetailsReturnsCorrectViewModelWhenViewModelIsNotNullAndUserIsOrgAdmin()
        {
            var organizationId = 100;
            var campaignId = 1;
            var controller = CampaignControllerWithDetailQuery(UserType.OrgAdmin.ToString(), organizationId);
            var result = await controller.Details(campaignId);

            var view = ValidateResult<ViewResult>(result);
            var model = ValidateResult<CampaignDetailModel>(view.ViewData.Model);
            Assert.Equal(model.OrganizationId, organizationId);
        }

        [Fact]
        public void CreateReturnsCorrectViewWithCorrectViewModel()
        {
            var sut = new CampaignController(null, null);
            var result = sut.Create();

            var view = ValidateResult<ViewResult>(result);
            var model = ValidateResult<CampaignSummaryModel>(view.ViewData.Model);
            Assert.Equal(view.ViewName, "Edit");
        }

        [Fact]
        public async Task EditGetSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
            var mediatorMock = new Mock<IMediator>();
            var sut = new CampaignController(mediatorMock.Object, null);
            var id = 2;
            var result = await sut.Edit(id);
            mediatorMock.Verify(mock => mock.SendAsync(It.Is<CampaignSummaryQuery>(cs => cs.CampaignId == id)), Times.Once);
        }

        [Fact]
        public async Task EditGetReturnsHttpNotFoundResultWhenViewModelIsNull()
        {
            CampaignController controller;
            MockMediatorCampaignSummaryQuery(out controller);
            Assert.IsType<HttpNotFoundResult>(await controller.Edit(It.IsAny<int>()));
        }

        [Fact]
        public async Task EditGetReturnsHttpUnauthorizedResultWhenUserIsNotAnOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            Assert.IsType<HttpUnauthorizedResult>(await controller.Edit(It.IsAny<int>()));
        }

        [Fact]
        public async Task EditGetReturnsCorrectViewModelWhenUserIsOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            Assert.IsType<ViewResult>(await controller.Edit(It.IsAny<int>()));
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

            var sut = new CampaignController(mockMediator.Object, mockImageService.Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            var file = FormFile("image/jpeg");

            await sut.Edit(new CampaignSummaryModel { Name = "Foo", OrganizationId = organizationId, Id = campaignId}, file);

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
        public async Task DeleteSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact]
        public async Task DeleteReturnsHttpNotFoundResultWhenCampaignIsNotFound()
        {
            CampaignController controller;
            MockMediatorCampaignSummaryQuery(out controller);
            Assert.IsType<HttpNotFoundResult>(await controller.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());   
            Assert.IsType<HttpUnauthorizedResult>(await controller.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteReturnsCorrectViewWhenUserIsOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            Assert.IsType<ViewResult>(await controller.Delete(It.IsAny<int>()));
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteReturnsCorrectViewModelWhenUserIsOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        public async Task DeleteConfirmedSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
            const int campaignId = 1;

            var mediator = new Mock<IMediator>();
            var sut = new CampaignController(mediator.Object, null);
            await sut.DeleteConfirmed(campaignId);

            mediator.Verify(mock => mock.SendAsync(It.Is<CampaignSummaryQuery>(i => i.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task DetailConfirmedReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            Assert.IsType<HttpUnauthorizedResult>(await controller.DeleteConfirmed(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailConfirmedSendsDeleteCampaignCommandWithCorrectCampaignIdWhenUserIsOrgAdmin()
        {
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryModel { OrganizationId = organizationId });

            var sut = new CampaignController(mockMediator.Object, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            await sut.DeleteConfirmed(campaignId);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<DeleteCampaignCommand>(i => i.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task DetailConfirmedRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsOrgAdmin()
        {
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryModel { OrganizationId = organizationId });
            
            var sut = new CampaignController(mockMediator.Object, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            var routeValues = new Dictionary<string, object> { ["area"] = "Admin" };

            var result = await sut.DeleteConfirmed(campaignId) as RedirectToActionResult;
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
        public async Task LockUnlockReturnsHttpUnauthorizedResultWhenUserIsNotSiteAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task LockUnlockSendsLockUnlockCampaignCommandWithCorrectCampaignIdWhenUserIsSiteAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task LockUnlockRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsSiteAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void LockUnlockHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void LockUnlockdHasValidateAntiForgeryTokenAttribute()
        {
        }

        #region Helper Methods
        private static Mock<IMediator> MockMediatorCampaignDetailQuery(out CampaignController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignDetailQuery>())).ReturnsAsync(null).Verifiable();

            var mockImageService = new Mock<IImageService>();

            controller = new CampaignController(mockMediator.Object, mockImageService.Object);

            return mockMediator;
        }

        private static Mock<IMediator> MockMediatorCampaignSummaryQuery(out CampaignController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(null).Verifiable();

            var mockImageService = new Mock<IImageService>();
            controller = new CampaignController(mockMediator.Object, mockImageService.Object);
            return mockMediator;
        }

        private static CampaignController CampaignControllerWithDetailQuery(string userType, int organizationId)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignDetailQuery>())).ReturnsAsync(new CampaignDetailModel { OrganizationId = organizationId }).Verifiable();

            var mockImageService = new Mock<IImageService>();

            var controller = new CampaignController(mockMediator.Object, mockImageService.Object);
            controller.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, userType),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            return controller;
        }

        private static CampaignController CampaignControllerWithSummaryQuery(string userType, int organizationId)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>()))
                .ReturnsAsync(new CampaignSummaryModel { OrganizationId = organizationId, Location = new LocationEditModel() }).Verifiable();

            var mockImageService = new Mock<IImageService>();

            var controller = new CampaignController(mockMediator.Object, mockImageService.Object);
            controller.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, userType),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            return controller;
        }

        private static CampaignController CampaignControllerWithCampaignListQuery(string userType, int organizationId, List<CampaignSummaryModel> campaignSummaries)
        {
            var mockMediator = new Mock<IMediator>();
           
            mockMediator.Setup(mock => mock.Send(It.IsAny<CampaignListQuery>())).Returns((CampaignListQuery c) => {

                    return c.OrganizationId.HasValue ? campaignSummaries.Where(cs => cs.OrganizationId == c.OrganizationId.Value).ToList() : campaignSummaries;
                });

            var mockImageService = new Mock<IImageService>();
            
            var controller = new CampaignController(mockMediator.Object, mockImageService.Object);
            controller.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, userType),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            return controller;
        }


        private static IFormFile FormFile(string fileType)
        {
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(mock => mock.ContentType).Returns(fileType);
            return mockFormFile.Object;
        }

        private T ValidateResult<T>(object result) where T : class
        {
            Assert.NotNull(result);
            Assert.IsType<T>(result);

            return result as T;
        }

        #endregion
    }
}
