using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using AllReady.Services;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using AllReady.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Areas.Admin.ViewModels.Organization;
using AllReady.Areas.Admin.ViewModels.Shared;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class CampaignAdminControllerTests 
    {
        //delete this line when all unit tests using it have been completed
        private static readonly Task<int> TaskFromResultZero = Task.FromResult(0);

        [Fact]
        public void IndexSendsCampaignListQueryWithCorrectData_WhenUserIsOrgAdmin()
        {
            const int organizationId = 99;
            var mockMediator = new Mock<IMediator>();

            var controller = new CampaignController(mockMediator.Object, null);
            controller.MakeUserAnOrgAdmin(organizationId.ToString());
            controller.Index();

            mockMediator.Verify(mock => mock.Send(It.Is<CampaignListQuery>(q => q.OrganizationId == organizationId)));
        }

        [Fact]
        public void IndexSendsCampaignListQueryWithCorrectData_WhenUserIsNotOrgAdmin()
        {
            var mockMediator = new Mock<IMediator>();

            var controller = new CampaignController(mockMediator.Object, null);
            controller.MakeUserNotAnOrgAdmin();
            controller.Index();

            mockMediator.Verify(mock => mock.Send(It.Is<CampaignListQuery>(q => q.OrganizationId == null)));
        }

        [Fact]
        public async Task DetailsSendsCampaignDetailQueryWithCorrectCampaignId()
        {
            const int campaignId = 100;
            const int organizationId = 99;
            var mockMediator = new Mock<IMediator>();

            // model is not null
            mockMediator.Setup(mock => mock.SendAsync(It.Is<CampaignDetailQuery>(c => c.CampaignId == campaignId))).ReturnsAsync(new CampaignDetailViewModel { OrganizationId = organizationId, Id = campaignId }).Verifiable();

            var controller = new CampaignController(mockMediator.Object, null);
            controller.SetClaims(new List<Claim>()); // create a User for the controller
            await controller.Details(campaignId);
            mockMediator.Verify(mock => mock.SendAsync(It.Is<CampaignDetailQuery>(c => c.CampaignId == campaignId)));
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundResultWhenVieModelIsNull()
        {
            CampaignController controller;
            MockMediatorCampaignDetailQuery(out controller);
            Assert.IsType<NotFoundResult>(await controller.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsHttpUnauthorizedResultIfUserIsNotOrgAdmin()
        {
            var controller = CampaignControllerWithDetailQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            Assert.IsType<UnauthorizedResult>(await controller.Details(It.IsAny<int>()));
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
            const int campaignId = 100;
            const int organizationId = 99;
            var mockMediator = new Mock<IMediator>();

            // model is not null
            mockMediator.Setup(mock => mock.SendAsync(It.Is<CampaignDetailQuery>(c=>c.CampaignId == campaignId))).ReturnsAsync(new CampaignDetailViewModel { OrganizationId = organizationId, Id = campaignId }).Verifiable();

            // user is org admin
            var controller = new CampaignController(mockMediator.Object, null);
            controller.MakeUserAnOrgAdmin(organizationId.ToString());

            var view = (ViewResult)(await controller.Details(campaignId));
            var viewModel = (CampaignDetailViewModel)view.ViewData.Model;
            Assert.Equal(viewModel.Id, campaignId);
            Assert.Equal(viewModel.OrganizationId, organizationId);
        }

        [Fact]
        public void CreateReturnsCorrectViewWithCorrectViewModel()
        {
            var mockMediator = new Mock<IMediator>();
            var controller = new CampaignController(mockMediator.Object, null);
            var view = (ViewResult) controller.Create();
            var viewModel = (CampaignSummaryViewModel)view.ViewData.Model;
            Assert.Equal(view.ViewName, "Edit");
            Assert.NotNull(viewModel);
        }

        [Fact]
        public async Task EditGetSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
            const int campaignId = 100;
            var mockMediator = new Mock<IMediator>();

            // model is not null
            mockMediator.Setup(mock => mock.SendAsync(It.Is<CampaignSummaryQuery>(c => c.CampaignId == campaignId))).ReturnsAsync(new CampaignSummaryViewModel { Id = campaignId });

            var controller = new CampaignController(mockMediator.Object, null);
            controller.SetClaims(new List<Claim>()); // create a User for the controller
            var view = await controller.Edit(campaignId);
            mockMediator.Verify(mock => mock.SendAsync(It.Is<CampaignSummaryQuery>(c => c.CampaignId == campaignId)));
        }

        [Fact]
        public async Task EditGetReturnsHttpNotFoundResultWhenViewModelIsNull()
        {
            CampaignController controller;
            MockMediatorCampaignSummaryQuery(out controller);
            Assert.IsType<NotFoundResult>(await controller.Edit(It.IsAny<int>()));
        }

        [Fact]
        public async Task EditGetReturnsHttpUnauthorizedResultWhenUserIsNotAnOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            Assert.IsType<UnauthorizedResult>(await controller.Edit(It.IsAny<int>()));
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
        public async Task EditPostAddsCorrectKeyAndErrorMessageToModelStateWhenCampaignEndDateIsLessThanCampainStartDate()
        {
            var campaignSummaryModel = new CampaignSummaryViewModel { OrganizationId = 1, StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(-1)};

            var sut = new CampaignController(null, null);
            sut.MakeUserAnOrgAdmin(campaignSummaryModel.OrganizationId.ToString());

            await sut.Edit(campaignSummaryModel, null);
            var modelStateErrorCollection = sut.ModelState.GetErrorMessagesByKey(nameof(CampaignSummaryViewModel.EndDate));

            Assert.Equal(modelStateErrorCollection.Single().ErrorMessage, "The end date must fall on or after the start date.");
        }

        [Fact]
        public async Task EditPostInsertsCampaign()
        {
            const int OrganizationId = 99;
            const int NewCampaignId = 100;
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EditCampaignCommandAsync>()))
                .Returns((EditCampaignCommandAsync q) => Task.FromResult<int>(NewCampaignId) );

            var mockImageService = new Mock<IImageService>();
            var controller = new CampaignController(mockMediator.Object, mockImageService.Object);
            controller.MakeUserAnOrgAdmin(OrganizationId.ToString());

            var model = MassiveTrafficLightOutage_model;
            model.OrganizationId = OrganizationId;

            // verify the model is valid
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults);
            Assert.Equal(0, validationResults.Count());

            var file = FormFile("image/jpeg");
            var view = (RedirectToActionResult) await controller.Edit(model, file);

            // verify the edit(add) is called
            mockMediator.Verify(mock => mock.SendAsync(It.Is<EditCampaignCommandAsync>(c => c.Campaign.OrganizationId == OrganizationId)));

            // verify that the next route
            Assert.Equal(view.RouteValues["area"], "Admin");
            Assert.Equal(view.RouteValues["id"], NewCampaignId);
        }

        [Fact]
        public async Task EditPostReturnsHttpUnauthorizedResultWhenUserIsNotAnOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            var result = await controller.Edit(new CampaignSummaryViewModel { OrganizationId = It.IsAny<int>() }, null);
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task EditPostRedirectsToCorrectActionWithCorrectRouteValuesWhenModelStateIsValid()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            var result = await controller.Edit(new CampaignSummaryViewModel { Name = "Foo", OrganizationId = It.IsAny<int>() }, null);

            //TODO: test result for correct Action name and Route values
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task EditPostAddsErrorToModelStateWhenInvalidImageFormatIsSupplied()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            var file = FormFile("");

            await controller.Edit(new CampaignSummaryViewModel { Name = "Foo", OrganizationId = It.IsAny<int>() }, file);

            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("ImageUrl"));
            //TODO: test that the value associated with the key is correct
        }

        [Fact]
        public async Task EditPostReturnsCorrectViewModelWhenInvalidImageFormatIsSupplied()
        {
            const int organizationId = 100;
            var mockMediator = new Mock<IMediator>();
            var mockImageService = new Mock<IImageService>();

            var sut = new CampaignController(mockMediator.Object, mockImageService.Object);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var file = FormFile("audio/mpeg3");
            var model = MassiveTrafficLightOutage_model;
            model.OrganizationId = organizationId;

            var view = await sut.Edit(model, file) as ViewResult;
            var viewModel = view.ViewData.Model as CampaignSummaryViewModel;
            Assert.True(ReferenceEquals(model, viewModel));
        }

        [Fact]
        public async Task EditPostInvokesUploadCampaignImageAsyncWithTheCorrectParameters_WhenFileUploadIsNotNull_AndFileIsAcceptableContentType()
        {
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            var mockImageService = new Mock<IImageService>();

            var sut = new CampaignController(mockMediator.Object, mockImageService.Object);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var file = FormFile("image/jpeg");

            await sut.Edit(new CampaignSummaryViewModel { Name = "Foo", OrganizationId = organizationId, Id = campaignId}, file);

            mockImageService.Verify(mock => mock.UploadCampaignImageAsync(
                It.Is<int>(i => i == organizationId),
                It.Is<int>(i => i == campaignId),
                It.Is<IFormFile>(i => i == file)), Times.Once);
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostInvokesDeleteImageAsync_WhenCampaignHasAnImage()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostDoesNotInvokeDeleteImageAsync__WhenCampaignDoesNotHaveAnImage()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void EditPostHasHttpPostAttribute()
        {
            var attr = (HttpPostAttribute)typeof(CampaignController).GetMethod(nameof(CampaignController.Edit), new Type[] { typeof(CampaignSummaryViewModel), typeof(IFormFile) }).GetCustomAttribute(typeof(HttpPostAttribute));
            Assert.NotNull(attr);
        }

        [Fact]
        public void EditPostHasValidateAntiForgeryTokenttribute()
        {
            var attr = (ValidateAntiForgeryTokenAttribute)typeof(CampaignController).GetMethod(nameof(CampaignController.Edit), new Type[] { typeof(CampaignSummaryViewModel), typeof(IFormFile) }).GetCustomAttribute(typeof(ValidateAntiForgeryTokenAttribute));
            Assert.NotNull(attr);
        }

        [Fact]
        public async Task DeleteSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
            const int organizationId = 99;
            const int campaignId = 100;
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.Is<CampaignSummaryQuery>(c => c.CampaignId == campaignId))).ReturnsAsync(new CampaignSummaryViewModel { Id = campaignId, OrganizationId = organizationId });
            var controller = new CampaignController(mockMediator.Object, null);
            controller.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });
            var view = (ViewResult)(await controller.Delete(campaignId));
            mockMediator.Verify(mock => mock.SendAsync(It.Is<CampaignSummaryQuery>(c => c.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task DeleteReturnsHttpNotFoundResultWhenCampaignIsNotFound()
        {
            CampaignController controller;
            MockMediatorCampaignSummaryQuery(out controller);
            Assert.IsType<NotFoundResult>(await controller.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());   
            Assert.IsType<UnauthorizedResult>(await controller.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteReturnsCorrectViewWhenUserIsOrgAdmin()
        {
            var controller = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            Assert.IsType<ViewResult>(await controller.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteReturnsCorrectViewModelWhenUserIsOrgAdmin()
        {
            const int organizationId = 99;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.Is<CampaignSummaryQuery>(c => c.CampaignId == campaignId))).ReturnsAsync(new CampaignSummaryViewModel { Id = campaignId, OrganizationId = organizationId });

            var controller = new CampaignController(mockMediator.Object, null);
            controller.MakeUserAnOrgAdmin(organizationId.ToString());

            var view = (ViewResult)await controller.Delete(campaignId);
            var viewModel = (CampaignSummaryViewModel)view.ViewData.Model;

            Assert.Equal(viewModel.Id, campaignId);
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
            Assert.IsType<UnauthorizedResult>(await controller.DeleteConfirmed(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailConfirmedSendsDeleteCampaignCommandWithCorrectCampaignIdWhenUserIsOrgAdmin()
        {
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel { OrganizationId = organizationId });

            var sut = new CampaignController(mockMediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            await sut.DeleteConfirmed(campaignId);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<DeleteCampaignCommand>(i => i.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task DetailConfirmedRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsOrgAdmin()
        {
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel { OrganizationId = organizationId });

            var sut = new CampaignController(mockMediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var routeValues = new Dictionary<string, object> { ["area"] = "Admin" };

            var result = await sut.DeleteConfirmed(campaignId) as RedirectToActionResult;
            Assert.Equal(result.ActionName, nameof(CampaignController.Index));
            Assert.Equal(result.RouteValues, routeValues);
        }

        [Fact]
        public void DeleteConfirmedHasHttpPostAttribute()
        {
            var attr = (HttpPostAttribute)typeof(CampaignController).GetMethod(nameof(CampaignController.DeleteConfirmed), new Type[] { typeof(int) }).GetCustomAttribute(typeof(HttpPostAttribute));
            Assert.NotNull(attr);
        }

        [Fact]
        public void DeleteConfirmedHasActionNameAttributeWithCorrectName()
        {
            var attr = (ActionNameAttribute)typeof(CampaignController).GetMethod(nameof(CampaignController.DeleteConfirmed), new Type[] { typeof(int) }).GetCustomAttribute(typeof(ActionNameAttribute));
            Assert.Equal(attr.Name, "Delete");
        }

        [Fact]
        public void DeleteConfirmedHasValidateAntiForgeryTokenAttribute()
        {
            var attr = (ValidateAntiForgeryTokenAttribute)typeof(CampaignController).GetMethod(nameof(CampaignController.DeleteConfirmed), new Type[] { typeof(int) }).GetCustomAttribute(typeof(ValidateAntiForgeryTokenAttribute));
            Assert.NotNull(attr);
        }

        [Fact]
        public async Task LockUnlockReturnsHttpUnauthorizedResultWhenUserIsNotSiteAdmin()
        {
            var controller = new CampaignController(null, null);
            controller.SetClaims(new List<Claim> { new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString()) });
            Assert.IsType<UnauthorizedResult>(await controller.LockUnlock(100));
        }

        [Fact]
        public async Task LockUnlockSendsLockUnlockCampaignCommandWithCorrectCampaignIdWhenUserIsSiteAdmin()
        {
            const int campaignId = 99;
            var mockMediator = new Mock<IMediator>();
            var controller = new CampaignController(mockMediator.Object, null);
            var claims = new List<Claim> { new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()) };
            controller.SetClaims(claims);

            await controller.LockUnlock(campaignId);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<LockUnlockCampaignCommand>(q => q.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task LockUnlockRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsSiteAdmin()
        {
            var CAMPAIGN_ID = 100;
            var mockMediator = new Mock<IMediator>();

            var controller = new CampaignController(mockMediator.Object, null);
            var claims = new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
            };
            controller.SetClaims(claims);

            var view = (RedirectToActionResult)await controller.LockUnlock(CAMPAIGN_ID);

            // verify the next route
            Assert.Equal(view.ActionName, nameof(CampaignController.Details));
            Assert.Equal(view.RouteValues["area"], "Admin");
            Assert.Equal(view.RouteValues["id"], CAMPAIGN_ID);
        }

        [Fact]
        public void LockUnlockHasHttpPostAttribute()
        {
            var attr = (HttpPostAttribute)typeof(CampaignController).GetMethod(nameof(CampaignController.LockUnlock), new Type[] { typeof(int) }).GetCustomAttribute(typeof(HttpPostAttribute));
            Assert.NotNull(attr);
        }

        [Fact]
        public void LockUnlockdHasValidateAntiForgeryTokenAttribute()
        {
            var attr = (ValidateAntiForgeryTokenAttribute)typeof(CampaignController).GetMethod(nameof(CampaignController.LockUnlock), new Type[] { typeof(int) }).GetCustomAttribute(typeof(ValidateAntiForgeryTokenAttribute));
            Assert.NotNull(attr);
        }

        private static Mock<IMediator> MockMediatorCampaignDetailQuery(out CampaignController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignDetailQuery>())).ReturnsAsync(null).Verifiable();

            controller = new CampaignController(mockMediator.Object, null);

            return mockMediator;
        }

        private static Mock<IMediator> MockMediatorCampaignSummaryQuery(out CampaignController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(null).Verifiable();

            controller = new CampaignController(mockMediator.Object, null);
            return mockMediator;
        }

        private static CampaignController CampaignControllerWithDetailQuery(string userType, int organizationId)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignDetailQuery>())).ReturnsAsync(new CampaignDetailViewModel { OrganizationId = organizationId }).Verifiable();

            var controller = new CampaignController(mockMediator.Object, null);
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
                .ReturnsAsync(new CampaignSummaryViewModel { OrganizationId = organizationId, Location = new LocationEditViewModel() }).Verifiable();

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

        public static LocationEditViewModel BogusAve_model => new LocationEditViewModel
        {
            Address1 = "25 Bogus Ave",
            City = "Agincourt",
            State = "Ontario",
            Country = "Canada",
            PostalCode = "M1T2T9"
        };

        public static OrganizationEditViewModel AgincourtAware_model => new OrganizationEditViewModel
        {
            Name = "Agincourt Awareness",
            Location = BogusAve_model,
            WebUrl = "http://www.AgincourtAwareness.ca",
            LogoUrl = "http://www.AgincourtAwareness.ca/assets/LogoLarge.png" }
        ;

        public static CampaignSummaryViewModel MassiveTrafficLightOutage_model => new CampaignSummaryViewModel
        {
            Description = "Preparations to be ready to deal with a wide-area traffic outage.",
            EndDate = DateTime.Today.AddMonths(1),
            ExternalUrl = "http://agincourtaware.trafficlightoutage.com",
            ExternalUrlText = "Agincourt Aware: Traffic Light Outage",
            Featured = false,
            FileUpload = null,
            FullDescription = "<h1><strong>Massive Traffic Light Outage Plan</strong></h1>\r\n<p>The Massive Traffic Light Outage Plan (MTLOP) is the official plan to handle a major traffic light failure.</p>\r\n<p>In the event of a wide-area traffic light outage, an alternative method of controlling traffic flow will be necessary. The MTLOP calls for the recruitment and training of volunteers to be ready to direct traffic at designated intersections and to schedule and follow-up with volunteers in the event of an outage.</p>",
            Id = 0,
            ImageUrl = null,
            Location = BogusAve_model,
            Locked = false,
            Name = "Massive Traffic Light Outage Plan",
            PrimaryContactEmail = "mlong@agincourtawareness.com",
            PrimaryContactFirstName = "Miles",
            PrimaryContactLastName = "Long",
            PrimaryContactPhoneNumber = "416-555-0119",
            StartDate = DateTime.Today,
            TimeZoneId = "Eastern Standard Time",
        };
    }
}