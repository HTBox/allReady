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
        public async Task IndexSendsIndexQueryWithCorrectData_WhenUserIsOrgAdmin()
        {
            const int organizationId = 99;
            var mockMediator = new Mock<IMediator>();

            var sut = new CampaignController(mockMediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());
            await sut.Index();

            mockMediator.Verify(mock => mock.SendAsync(It.Is<IndexQuery>(q => q.OrganizationId == organizationId)));
        }

        [Fact]
        public async Task IndexSendsIndexQueryWithCorrectData_WhenUserIsNotOrgAdmin()
        {
            var mockMediator = new Mock<IMediator>();

            var sut = new CampaignController(mockMediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();
            await sut.Index();

            mockMediator.Verify(mock => mock.SendAsync(It.Is<IndexQuery>(q => q.OrganizationId == null)));
        }

        [Fact]
        public async Task DetailsSendsCampaignDetailQueryWithCorrectCampaignId()
        {
            const int campaignId = 100;
            var mockMediator = new Mock<IMediator>();

            var sut = new CampaignController(mockMediator.Object, null);
            await sut.Details(campaignId);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<CampaignDetailQuery>(c => c.CampaignId == campaignId)));
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundResultWhenVieModelIsNull()
        {
            CampaignController sut;
            MockMediatorCampaignDetailQuery(out sut);
            Assert.IsType<NotFoundResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsHttpUnauthorizedResultIfUserIsNotOrgAdmin()
        {
            var sut = CampaignControllerWithDetailQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            Assert.IsType<UnauthorizedResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsCorrectViewWhenViewModelIsNotNullAndUserIsOrgAdmin()
        {
            var sut = CampaignControllerWithDetailQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            Assert.IsType<ViewResult>(await sut.Details(It.IsAny<int>()));
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
            var sut = new CampaignController(mockMediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var view = (ViewResult)(await sut.Details(campaignId));
            var viewModel = (CampaignDetailViewModel)view.ViewData.Model;
            Assert.Equal(viewModel.Id, campaignId);
            Assert.Equal(viewModel.OrganizationId, organizationId);
        }

        [Fact]
        public void CreateReturnsCorrectViewWithCorrectViewModel()
        {
            var sut = new CampaignController(Mock.Of<IMediator>(), null);
            var view = (ViewResult) sut.Create();

            var viewModel = (CampaignSummaryViewModel)view.ViewData.Model;

            Assert.Equal(view.ViewName, "Edit");
            Assert.NotNull(viewModel);
        }

        [Fact]
        public async Task EditGetSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
            const int campaignId = 100;
            var mockMediator = new Mock<IMediator>();

            var sut = new CampaignController(mockMediator.Object, null);
            await sut.Edit(campaignId);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<CampaignSummaryQuery>(c => c.CampaignId == campaignId)));
        }

        [Fact]
        public async Task EditGetReturnsHttpNotFoundResultWhenViewModelIsNull()
        {
            CampaignController sut;
            MockMediatorCampaignSummaryQuery(out sut);
            Assert.IsType<NotFoundResult>(await sut.Edit(It.IsAny<int>()));
        }

        [Fact]
        public async Task EditGetReturnsHttpUnauthorizedResultWhenUserIsNotAnOrgAdmin()
        {
            var sut = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            Assert.IsType<UnauthorizedResult>(await sut.Edit(It.IsAny<int>()));
        }

        [Fact]
        public async Task EditGetReturnsCorrectViewModelWhenUserIsOrgAdmin()
        {
            var sut = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            Assert.IsType<ViewResult>(await sut.Edit(It.IsAny<int>()));
        }

        [Fact]
        public async Task EditPostReturnsBadRequestWhenCampaignIsNull()
        {
            var sut = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            var result = await sut.Edit(null, null);
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
            const int organizationId = 99;
            const int newCampaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EditCampaignCommand>()))
                .Returns((EditCampaignCommand q) => Task.FromResult<int>(newCampaignId) );

            var mockImageService = new Mock<IImageService>();
            var sut = new CampaignController(mockMediator.Object, mockImageService.Object);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var model = MassiveTrafficLightOutageModel;
            model.OrganizationId = organizationId;

            // verify the model is valid
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults);
            Assert.Equal(0, validationResults.Count());

            var file = FormFile("image/jpeg");
            var view = (RedirectToActionResult) await sut.Edit(model, file);

            // verify the edit(add) is called
            mockMediator.Verify(mock => mock.SendAsync(It.Is<EditCampaignCommand>(c => c.Campaign.OrganizationId == organizationId)));

            // verify that the next route
            Assert.Equal(view.RouteValues["area"], "Admin");
            Assert.Equal(view.RouteValues["id"], newCampaignId);
        }

        [Fact]
        public async Task EditPostReturnsHttpUnauthorizedResultWhenUserIsNotAnOrgAdmin()
        {
            var sut = CampaignControllerWithSummaryQuery(UserType.BasicUser.ToString(), It.IsAny<int>());
            var result = await sut.Edit(new CampaignSummaryViewModel { OrganizationId = It.IsAny<int>() }, null);
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task EditPostRedirectsToCorrectActionWithCorrectRouteValuesWhenModelStateIsValid()
        {
            var sut = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            var result = await sut.Edit(new CampaignSummaryViewModel { Name = "Foo", OrganizationId = It.IsAny<int>() }, null);

            //TODO: test result for correct Action name and Route values
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task EditPostAddsErrorToModelStateWhenInvalidImageFormatIsSupplied()
        {
            var sut = CampaignControllerWithSummaryQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            var file = FormFile("");

            await sut.Edit(new CampaignSummaryViewModel { Name = "Foo", OrganizationId = It.IsAny<int>() }, file);

            Assert.False(sut.ModelState.IsValid);
            Assert.True(sut.ModelState.ContainsKey("ImageUrl"));
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
            var model = MassiveTrafficLightOutageModel;
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
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            var mockImageService = new Mock<IImageService>();

            var sut = new CampaignController(mockMediator.Object, mockImageService.Object);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var file = FormFile("image/jpeg");
            var campaignSummaryViewModel = new CampaignSummaryViewModel
            {
                Name = "Foo",
                OrganizationId = organizationId,
                Id = campaignId,
                StartDate = new DateTimeOffset(new DateTime(2016, 2, 13)),
                EndDate = new DateTimeOffset(new DateTime(2016, 2, 14)),
            };

            await sut.Edit(campaignSummaryViewModel, file);

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
        public async Task DeleteSendsDeleteQueryWithCorrectCampaignId()
        {
            const int organizationId = 99;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.Is<DeleteViewModelQuery>(c => c.CampaignId == campaignId))).ReturnsAsync(new DeleteViewModel { Id = campaignId, OrganizationId = organizationId });

            var sut = new CampaignController(mockMediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());
            await sut.Delete(campaignId);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<DeleteViewModelQuery>(c => c.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task DeleteReturnsHttpNotFoundResultWhenCampaignIsNotFound()
        {
            CampaignController sut;
            MockMediatorCampaignSummaryQuery(out sut);
            Assert.IsType<NotFoundResult>(await sut.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteViewModelQuery>())).ReturnsAsync(new DeleteViewModel());

            var sut = new CampaignController(mediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteReturnsCorrectViewWhenUserIsOrgAdmin()
        {
            const int organizationId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteViewModelQuery>())).ReturnsAsync(new DeleteViewModel { OrganizationId = organizationId });

            var sut = new CampaignController(mediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            Assert.IsType<ViewResult>(await sut.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteReturnsCorrectViewModelWhenUserIsOrgAdmin()
        {
            const int organizationId = 99;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.Is<DeleteViewModelQuery>(c => c.CampaignId == campaignId))).ReturnsAsync(new DeleteViewModel { Id = campaignId, OrganizationId = organizationId });

            var sut = new CampaignController(mockMediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var view = (ViewResult)await sut.Delete(campaignId);
            var viewModel = (DeleteViewModel)view.ViewData.Model;

            Assert.Equal(viewModel.Id, campaignId);
        }

        public async Task DeleteConfirmedSendsDeleteCampaignCommandWithCorrectCampaignId()
        {
            var viewModel = new DeleteViewModel { Id = 1 };

            var mediator = new Mock<IMediator>();
            var sut = new CampaignController(mediator.Object, null);
            await sut.DeleteConfirmed(viewModel);

            mediator.Verify(mock => mock.SendAsync(It.Is<DeleteCampaignCommand>(i => i.CampaignId == viewModel.Id)), Times.Once);
        }

        [Fact]
        public async Task DetailConfirmedReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var sut = new CampaignController(Mock.Of<IMediator>(), null);
            Assert.IsType<UnauthorizedResult>(await sut.DeleteConfirmed(new DeleteViewModel { UserIsOrgAdmin = false }));
        }

        [Fact]
        public async Task DetailConfirmedSendsDeleteCampaignCommandWithCorrectCampaignIdWhenUserIsOrgAdmin()
        {
            const int organizationId = 1;

            var mockMediator = new Mock<IMediator>();

            var viewModel = new DeleteViewModel { Id = 100, UserIsOrgAdmin = true };

            var sut = new CampaignController(mockMediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            await sut.DeleteConfirmed(viewModel);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<DeleteCampaignCommand>(i => i.CampaignId == viewModel.Id)), Times.Once);
        }

        [Fact]
        public async Task DetailConfirmedRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsOrgAdmin()
        {
            const int organizationId = 1;

            var viewModel = new DeleteViewModel { Id = 100, UserIsOrgAdmin = true };

            var sut = new CampaignController(Mock.Of<IMediator>(), null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var routeValues = new Dictionary<string, object> { ["area"] = "Admin" };

            var result = await sut.DeleteConfirmed(viewModel) as RedirectToActionResult;
            Assert.Equal(result.ActionName, nameof(CampaignController.Index));
            Assert.Equal(result.RouteValues, routeValues);
        }

        [Fact]
        public void DeleteConfirmedHasHttpPostAttribute()
        {
            var sut = CreateCampaignControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<DeleteViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DeleteConfirmedHasActionNameAttributeWithCorrectName()
        {
            var sut = CreateCampaignControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<DeleteViewModel>())).OfType<ActionNameAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Name, "Delete");
        }

        [Fact]
        public void DeleteConfirmedHasValidateAntiForgeryTokenAttribute()
        {
            var sut = CreateCampaignControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<DeleteViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task LockUnlockReturnsHttpUnauthorizedResultWhenUserIsNotSiteAdmin()
        {
            var sut = CreateCampaignControllerWithNoInjectedDependencies();
            sut.MakeUserAnOrgAdmin("1");
            Assert.IsType<UnauthorizedResult>(await sut.LockUnlock(100));
        }

        [Fact]
        public async Task LockUnlockSendsLockUnlockCampaignCommandWithCorrectCampaignIdWhenUserIsSiteAdmin()
        {
            const int campaignId = 99;
            var mockMediator = new Mock<IMediator>();

            var sut = new CampaignController(mockMediator.Object, null);
            sut.MakeUserASiteAdmin();

            await sut.LockUnlock(campaignId);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<LockUnlockCampaignCommand>(q => q.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task LockUnlockRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsSiteAdmin()
        {
            const int campaignId = 100;
            var mockMediator = new Mock<IMediator>();

            var sut = new CampaignController(mockMediator.Object, null);
            sut.MakeUserASiteAdmin();

            var view = (RedirectToActionResult)await sut.LockUnlock(campaignId);

            // verify the next route
            Assert.Equal(view.ActionName, nameof(CampaignController.Details));
            Assert.Equal(view.RouteValues["area"], "Admin");
            Assert.Equal(view.RouteValues["id"], campaignId);
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

        private static CampaignController CreateCampaignControllerWithNoInjectedDependencies()
        {
            return new CampaignController(null, null);
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

            var sut = new CampaignController(mockMediator.Object, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, userType),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            return sut;
        }

        private static CampaignController CampaignControllerWithSummaryQuery(string userType, int organizationId)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>()))
                .ReturnsAsync(new CampaignSummaryViewModel { OrganizationId = organizationId, Location = new LocationEditViewModel() }).Verifiable();

            var mockImageService = new Mock<IImageService>();

            var sut = new CampaignController(mockMediator.Object, mockImageService.Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, userType),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            return sut;
        }

        private static IFormFile FormFile(string fileType)
        {
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(mock => mock.ContentType).Returns(fileType);
            return mockFormFile.Object;
        }

        private static LocationEditViewModel BogusAveModel => new LocationEditViewModel
        {
            Address1 = "25 Bogus Ave",
            City = "Agincourt",
            State = "Ontario",
            Country = "Canada",
            PostalCode = "M1T2T9"
        };

        private static OrganizationEditViewModel AgincourtAwareModel => new OrganizationEditViewModel
        {
            Name = "Agincourt Awareness",
            Location = BogusAveModel,
            WebUrl = "http://www.AgincourtAwareness.ca",
            LogoUrl = "http://www.AgincourtAwareness.ca/assets/LogoLarge.png" }
        ;

        private static CampaignSummaryViewModel MassiveTrafficLightOutageModel => new CampaignSummaryViewModel
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
            Location = BogusAveModel,
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