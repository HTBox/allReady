using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using AllReady.Services;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Linq;
using System.Reflection;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Areas.Admin.ViewModels.Campaign;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Shouldly;
using DeleteViewModel = AllReady.Areas.Admin.Features.Events.DeleteViewModel;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class EventAdminControllerTests
    {
        //delete this line when all unit tests using it have been completed
        private static readonly Task TaskCompletedTask = Task.CompletedTask;

        [Fact]
        public async Task DetailsReturnsHttpNotFoundResult_WhenEventIsNull()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null);
            var result = await sut.Details(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailsReturnsHttpUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync(new EventDetailViewModel { Id = 1, Name = "Itinerary", OrganizationId = 1 });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();
            Assert.IsType<UnauthorizedResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsHttpUnauthorizedResult_WhenEventIsNotNull_AndUserIsNotAnOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync(new EventDetailViewModel { Id = 1, Name = "Itinerary", OrganizationId = 1 });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();
            Assert.IsType<UnauthorizedResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsCorrectViewModel_WhenEventIsNotNull_AndUserIsOrgAdmin()
        {
            const int orgId = 1;
            const int eventID = 1;
            var viewModel = new EventDetailViewModel { Id = eventID, Name = "Itinerary", OrganizationId = orgId };
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<EventDetailQuery>(q => q.EventId == eventID))).ReturnsAsync(viewModel);

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper
                .Setup(url => url.Action(It.IsAny<UrlActionContext>()))
                .Returns("baseUrl/");

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());
            sut.Url = mockUrlHelper.Object;

            var result = await sut.Details(eventID) as ViewResult;
            Assert.Equal(result.ViewName, null);

            var resultViewModel = result.ViewData.Model;
            Assert.IsType<EventDetailViewModel>(resultViewModel);
            Assert.Equal(resultViewModel, viewModel);
        }

        [Fact]
        public void DetailsHasHttpGetAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DetailsHasRouteAttributeWithCorrectRoute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var routeAttribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Event/Details/{id}");
        }

        [Fact]
        public async Task CreateGetSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
            const int campaignId = 99;
            var mediator = new Mock<IMediator>();

            var sut = new EventController(null, mediator.Object, null);
            await sut.Create(campaignId);

            mediator.Verify(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>()), Times.Once);
            mediator.Verify(m => m.SendAsync(It.Is<CampaignSummaryQuery>(q => q.CampaignId == campaignId)));
        }

        [Fact]
        public async Task CreateGetReturnsHttpUnauthorizedResult_WhenCampaignIsNull()
        {
            var mediator = new Mock<IMediator>();
            var sut = new EventController(null, mediator.Object, null);

            Assert.IsType<UnauthorizedResult>(await sut.Create(It.IsAny<int>()));
        }

        [Fact]
        public async Task CreateGetReturnsHttpUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>()))
				.ReturnsAsync(new EventDetailViewModel { Id = 1, Name = "Itinerary", OrganizationId = 1 });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.Create(It.IsAny<int>()));
        }

        [Fact]
		public async Task CreateGetReturnsCorrectView_AndCorrectStartAndEndDateOnViewModel()
        {
            const int organizationId = 1;
            var dateTimeTodayDate = DateTime.Today.Date;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CampaignSummaryQuery>()))
                .ReturnsAsync(new CampaignSummaryViewModel { OrganizationId = organizationId });

            var sut = new EventController(null, mediator.Object, null) { DateTimeTodayDate = () => dateTimeTodayDate };
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var view = await sut.Create(It.IsAny<int>()) as ViewResult;
            var viewModel = view.ViewData.Model as EventEditViewModel;

            Assert.Equal(view.ViewName, "Edit");
            Assert.Equal(viewModel.StartDateTime, dateTimeTodayDate);
            Assert.Equal(viewModel.EndDateTime, dateTimeTodayDate);
        }

        [Fact]
        public async Task CreateGetReturnsCorrectView_AndCorrectCampaignID()
        {
            const int orgId = 1;
            const int campaignId = 99;

            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.SendAsync(It.Is<CampaignSummaryQuery>(q => q.CampaignId == campaignId)))
                .ReturnsAsync(new CampaignSummaryViewModel { Id = campaignId, OrganizationId = orgId });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.Create(campaignId) as ViewResult;

            Assert.Equal(result.ViewName, "Edit");
            var resultViewModel = result.ViewData.Model;
            Assert.IsType<EventEditViewModel>(resultViewModel);

            //
            Assert.Equal((resultViewModel as EventEditViewModel).CampaignId, campaignId);
        }

        [Fact]
        public void CreateGetHasRouteAttributeWithCorrectRoute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var routeAttribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Event/Create/{campaignId}");
        }

        [Fact]
        public async Task CreatePostReturnsEditView_When_ModelStateNotValid()
        {
            var imageService = new Mock<IImageService>();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel());

            var eventDetailModelValidator = new Mock<IValidateEventEditViewModels>();
            eventDetailModelValidator.Setup(x => x.Validate(It.IsAny<EventEditViewModel>(), It.IsAny<CampaignSummaryViewModel>()))
                .Returns(new List<KeyValuePair<string, string>>());

            var sut = new EventController(imageService.Object, mediator.Object, eventDetailModelValidator.Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            sut.ModelState.AddModelError("test", "test");
            var result = (ViewResult)await sut.Create(It.IsAny<int>(), It.IsAny<EventEditViewModel>(), null);

            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
		public async Task CreatePostReturnsEditView_When_EventDetailsModelValidatorHasErrors()
        {
            var imageService = new Mock<IImageService>();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel());

            var eventDetailModelValidator = new Mock<IValidateEventEditViewModels>();
            eventDetailModelValidator.Setup(x => x.Validate(It.IsAny<EventEditViewModel>(), It.IsAny<CampaignSummaryViewModel>()))
                .Returns(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("ErrorKey", "ErrorMessage") });

            var sut = new EventController(imageService.Object, mediator.Object, eventDetailModelValidator.Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            var result = (ViewResult)await sut.Create(1, It.IsAny<EventEditViewModel>(), null);
            Assert.Equal("Edit", result.ViewName);
        }

        /// <summary>
        /// The following unit test fails because of a bug (#3586) in MVC 6 that breaks TryValidateModel().
        /// The bug is fixed in RC2. TODO: uncomment the below test once MVC reference is updated to RC2.
        /// </summary>
        /// <returns></returns>
        //[Fact]
        //public async Task CreatePostReturnsEditView_When_ModelStateNotValid_And_ImageIsNotNull()
        //{
        //    var sut = GetEventController();
        //    var eventModel = new EventDetailViewModel();
        //    IFormFile file = new FormFile(null, 0, 0);
        //    var result = (ViewResult)await sut.Create(1, eventModel, file);

        //    Assert.Equal("Edit", result.ViewName);
        //}

        [Fact]
        public void CreatePostHasHttpPostAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>(), It.IsAny<EventEditViewModel>(), It.IsAny<IFormFile>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void CreatePostHasValidateAntiForgeryTokenAttrbiute()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null);
            var routeAttribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>(), It.IsAny<EventEditViewModel>(), It.IsAny<IFormFile>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact]
        public void CreatePostHasRouteAttrbiuteWithCorrectRoute()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null);
            var routeAttribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>(), It.IsAny<EventEditViewModel>(), It.IsAny<IFormFile>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Event/Create/{campaignId}");
        }

        [Fact]
        public async void EditGetSendsEventDetailQueryWithCorrectEventId()
        {
            const int orgId = 1;
            const int eventId = 100;
            EventEditViewModel viewModel = new EventEditViewModel { Id = eventId };

            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.SendAsync(It.Is<EventEditQuery>(q => q.EventId == eventId)))
                .ReturnsAsync(viewModel);

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            await sut.Edit(eventId);

            mediator.Verify(m => m.SendAsync(It.IsAny<EventEditQuery>()), Times.Once);
            mediator.Verify(m => m.SendAsync(It.Is<EventEditQuery>(q => q.EventId == eventId)));
        }

        [Fact]
        public async void EditGetReturnsHttpNotFoundResult_WhenEventIsNull()
        {
            var mediator = new Mock<IMediator>();
            var sut = new EventController(null, mediator.Object, null);
            var result = await sut.Edit(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void EditGetReturnsHttpUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync(new EventEditViewModel { Id = 1, Name = "Itinerary", OrganizationId = 1 });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();
            Assert.IsType<UnauthorizedResult>(await sut.Edit(It.IsAny<int>()));
        }

        [Fact]
        public async void EditGetReturnsCorrectViewModel()
        {
            const int orgId = 1;
            const int eventId = 100;
            EventEditViewModel viewModel = new EventEditViewModel { Id = eventId, OrganizationId = orgId };

            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.SendAsync(It.Is<EventEditQuery>(q => q.EventId == eventId)))
                .ReturnsAsync(viewModel);

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.Edit(eventId) as ViewResult;
            Assert.IsType<EventEditViewModel>(result.ViewData.Model);
        }

        [Fact]
        public async Task EditPostReturnsBadRequestResult_WhenEventIsNull()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null);
            Assert.IsType<BadRequestResult>(await sut.Edit(It.IsAny<EventEditViewModel>(), It.IsAny<IFormFile>()));
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostSendsManagingOrganizationIdByEventIdQueryWithCorrectEventId()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostReturnsHttpUnauthorizedResult_WhenUserIsNotOrgAdminUser()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostSendsCampaignSummaryQueryWithTheCorrectCampaignId()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostAddsValidationErrorsToModelStateErrors_WhenEventDetailsModelValidatorHasErrors()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostReturnsCorrectView_WhenEventDetailsModelValidatorHasErrors()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostInvokesUploadEventImageAsyncWithTheCorrectParameters_WhenModelStateIsValid_AndFileUploadIsNotNull_AndFileUploadIsAnAcceptableImageContentType()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostInvokesDeleteImageAsyncWithTheCorrectParameters_WhenModelStateIsValid_AndFileUploadIsNotNull_AndFileUploadIsAnAcceptableImageContentType_AndThereIsAnExistingImage()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostDoesNotInvokeDeleteImageAsyncWithTheCorrectParameters_WhenModelStateIsValid_AndFileUploadIsNotNull_AndFileUploadIsAnAcceptableImageContentType_AndThereIsAnExistingImage()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostAddsCorrectKey_AndValueToModelStateErrors_WhenModelStateIsValid_AndFileUploadIsNotNull_AndFileUploadIsNotAnAcceptableImageContentType()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostReturnsCorrectViewModel_WhenModelStateIsValid_AndFileUploadIsNotNull_AndFileUploadIsNotAnAcceptableImageContentType()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostSendsEditEventCommandWithCorrectEvent_WhenModelStateIsValid()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostRedirectsToCorrectAction_AndControllerWithCorrectRouteValues_WhenModelStateIsValid()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostReturnsCorrectViewModel_WhenModelStateIsNotValid()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact]
        public void EditPostHasHttpPostAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<EventEditViewModel>(), It.IsAny<IFormFile>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void EditPostHasValidateAntiForgeryTokenAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var routeAttribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<EventEditViewModel>(), It.IsAny<IFormFile>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteGetSendsDeleteQueryWithCorrectEventId()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact]
        public async Task DeleteGetReturnsHttpUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteQuery>()))
                .ReturnsAsync(new DeleteViewModel());

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteGetSetsUserIsOrgAdminToTrueOnTheViewModel_WhenUserIsOrgAdmin()
        {
            const int organizationId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteQuery>())).ReturnsAsync(new DeleteViewModel { OrganizationId = organizationId });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var result = await sut.Delete(It.IsAny<int>()) as ViewResult;
            var resultModel = result.ViewData.Model as DeleteViewModel;

            Assert.True(resultModel.UserIsOrgAdmin);
        }

        [Fact]
        public async Task DeleteGetSetsTheCorrectTitleOnTheViewModel()
        {
            const string eventName = "EventName";
            const int organizationId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteQuery>())).ReturnsAsync(new DeleteViewModel { Name = eventName, OrganizationId = organizationId });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var result = await sut.Delete(It.IsAny<int>()) as ViewResult;
            var resultModel = result.ViewData.Model as DeleteViewModel;

            Assert.Equal(resultModel.Title, $"Delete event {eventName}");
        }

        [Fact]
        public async Task DeleteGetReturnsCorrectViewModel()
        {
            const int organizationId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteQuery>()))
                .ReturnsAsync(new DeleteViewModel { OrganizationId = organizationId });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var result = (ViewResult) await sut.Delete(It.IsAny<int>());
            var resultModel = result.ViewData.Model;

            Assert.IsType<DeleteViewModel>(resultModel);
        }

        [Fact]
        public void DeleteGetHasActionNameAttributeWithCorrectName()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Delete(It.IsAny<int>())).OfType<ActionNameAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Name, "Delete");
        }

        [Fact]
        public async Task DeleteConfirmedReturnsHttpUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            var sut = new EventController(null, null, null);
            Assert.IsType<UnauthorizedResult>(await sut.DeleteConfirmed(new DeleteViewModel { UserIsOrgAdmin = false }));
        }

        [Fact]
        public async Task DeleteConfirmedSendsDeleteEventCommandWithCorrectEventId()
        {
            const int eventId = 100;
            DeleteViewModel viewModel = new DeleteViewModel { Id = eventId, UserIsOrgAdmin = true };

            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.SendAsync(It.IsAny<DeleteEventCommand>()))
                .ReturnsAsync(It.IsAny<Unit>());

            var sut = new EventController(null, mediator.Object, null);

            await sut.DeleteConfirmed(viewModel);

            mediator.Verify(m => m.SendAsync(It.IsAny<DeleteEventCommand>()), Times.Once);
            mediator.Verify(m => m.SendAsync(It.Is<DeleteEventCommand>(c => c.EventId == eventId)));
        }

        [Fact]
        public async Task DeleteConfirmedRedirectToCorrectAction_AndControllerWithCorrectRouteValues()
        {
            const int eventId = 100;
            const int campaignId = 88;
            DeleteViewModel viewModel = new DeleteViewModel { Id = eventId, UserIsOrgAdmin = true, CampaignId = campaignId };

            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.SendAsync(It.IsAny<DeleteEventCommand>()))
                .ReturnsAsync(It.IsAny<Unit>());

            var sut = new EventController(null, mediator.Object, null);

            var result = await sut.DeleteConfirmed(viewModel) as RedirectToActionResult;

            Assert.Equal(result.ActionName, nameof(CampaignController.Details));
            Assert.Equal(result.ControllerName, "Campaign");
            Assert.Equal(result.RouteValues["area"], "Admin");
            Assert.Equal(result.RouteValues["id"], campaignId);
        }

        [Fact]
        public void DeleteConfirmedHasHttpPostAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<DeleteViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DeleteConfirmedHasActionNameAttributeWithCorrectName()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<DeleteViewModel>())).OfType<ActionNameAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Name, "Delete");
        }

        [Fact]
        public void DeleteConfirmedHasValidateAntiForgeryTokenAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var routeAttribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<DeleteViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact]
        public async Task DeleteEventImageReturnsJsonObjectWithStatusOfNotFound()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync(null);
            var imageServiceMock = new Mock<IImageService>();
            var eventEditViewModelValidatorMock = new Mock<IValidateEventEditViewModels>();
            var sut = new EventController(imageServiceMock.Object, mediatorMock.Object,  eventEditViewModelValidatorMock.Object);

            var result = await sut.DeleteEventImage(It.IsAny<int>());

            result.ShouldNotBeNull();
            result.ShouldBeOfType<JsonResult>();

            result.Value.GetType()
                .GetTypeInfo()
                .GetProperty("status")
                .GetValue(result.Value)
                .ShouldBe("NotFound");
        }


        [Fact]
        public async Task DeleteEventImageReturnsJsonObjectWithStatusOfUnauthorizedIfUserIsNotOrganizationAdmin()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync(new EventEditViewModel());
            var imageServiceMock = new Mock<IImageService>();
            var eventEditViewModelValidatorMock = new Mock<IValidateEventEditViewModels>();
            var sut = new EventController(imageServiceMock.Object, mediatorMock.Object, eventEditViewModelValidatorMock.Object);
            sut.MakeUserNotAnOrgAdmin();

            var result = await sut.DeleteEventImage(It.IsAny<int>());

            result.Value.GetType()
                .GetTypeInfo()
                .GetProperty("status")
                .GetValue(result.Value)
                .ShouldBe("Unauthorized");
        }


        [Fact]
        public async Task DeleteEventSendsTheCorrectIdToEventEditQuery()
        {
            var mediatorMock = new Mock<IMediator>();
            var imageServiceMock = new Mock<IImageService>();
            var eventEditViewModelValidatorMock = new Mock<IValidateEventEditViewModels>();
            var sut = new EventController(imageServiceMock.Object, mediatorMock.Object, eventEditViewModelValidatorMock.Object);

            const int eventId = 2;

            await sut.DeleteEventImage(eventId);

            mediatorMock.Verify(m => m.SendAsync(It.IsAny<EventEditQuery>()), Times.Once);
            mediatorMock.Verify(m => m.SendAsync(It.Is<EventEditQuery>(s => s.EventId == eventId)));
        }


        [Fact]
        public async Task DeleteEventImageCallsDeleteImageAsyncWithCorrectData()
        {
            var mediatorMock = new Mock<IMediator>();

            var eventEditViewModel = new EventEditViewModel
            {
                OrganizationId = 1,
                ImageUrl = "URL!"
            };

            mediatorMock.Setup(m => m.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync(eventEditViewModel);

            var imageServiceMock = new Mock<IImageService>();
            var eventEditViewModelValidatorMock = new Mock<IValidateEventEditViewModels>();
            var sut = new EventController(imageServiceMock.Object, mediatorMock.Object, eventEditViewModelValidatorMock.Object);
            sut.MakeUserAnOrgAdmin(eventEditViewModel.OrganizationId.ToString());

            await sut.DeleteEventImage(It.IsAny<int>());

            imageServiceMock.Verify(i => i.DeleteImageAsync(It.Is<string>(f => f == "URL!")), Times.Once);
        }


        [Fact]
        public async Task DeleteEventImageCallsEditEventCommandWithCorrectData()
        {
            var mediatorMock = new Mock<IMediator>();

            var eventEditViewModel = new EventEditViewModel
            {
                OrganizationId = 1,
                ImageUrl = "URL!"
            };

            mediatorMock.Setup(m => m.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync(eventEditViewModel);

            var imageServiceMock = new Mock<IImageService>();
            var eventEditViewModelValidatorMock = new Mock<IValidateEventEditViewModels>();
            var sut = new EventController(imageServiceMock.Object, mediatorMock.Object, eventEditViewModelValidatorMock.Object);
            sut.MakeUserAnOrgAdmin(eventEditViewModel.OrganizationId.ToString());

            await sut.DeleteEventImage(It.IsAny<int>());

            mediatorMock.Verify(m => m.SendAsync(It.Is<EditEventCommand>(s => s.Event == eventEditViewModel)), Times.Once);
        }


        [Fact]
        public async Task DeleteEventImageReturnsJsonObjectWithStatusOfSuccessIfImageDeletedSuccessfully()
        {
            var mediatorMock = new Mock<IMediator>();

            var eventEditViewModel = new EventEditViewModel
            {
                OrganizationId = 1,
                ImageUrl = "URL!"
            };

            mediatorMock.Setup(m => m.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync(eventEditViewModel);

            var imageServiceMock = new Mock<IImageService>();
            var eventEditViewModelValidatorMock = new Mock<IValidateEventEditViewModels>();
            var sut = new EventController(imageServiceMock.Object, mediatorMock.Object, eventEditViewModelValidatorMock.Object);
            sut.MakeUserAnOrgAdmin(eventEditViewModel.OrganizationId.ToString());

            var result = await sut.DeleteEventImage(It.IsAny<int>());

            result.Value.GetType()
                .GetTypeInfo()
                .GetProperty("status")
                .GetValue(result.Value)
                .ShouldBe("Success");
        }


        [Fact]
        public async Task DeleteEventImageReturnsJsonObjectWithStatusOfNothingToDeleteIfThereWasNoExistingImage()
        {
            var mediatorMock = new Mock<IMediator>();

            var eventEditViewModel = new EventEditViewModel
            {
                OrganizationId = 1
            };

            mediatorMock.Setup(m => m.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync(eventEditViewModel);

            var imageServiceMock = new Mock<IImageService>();
            var eventEditViewModelValidatorMock = new Mock<IValidateEventEditViewModels>();
            var sut = new EventController(imageServiceMock.Object, mediatorMock.Object, eventEditViewModelValidatorMock.Object);
            sut.MakeUserAnOrgAdmin(eventEditViewModel.OrganizationId.ToString());

            var result = await sut.DeleteEventImage(It.IsAny<int>());

            result.Value.GetType()
                .GetTypeInfo()
                .GetProperty("status")
                .GetValue(result.Value)
                .ShouldBe("NothingToDelete");
        }

        [Fact]
        public async Task MessageAllVolunteersReturnsBadRequestObjectResult_WhenModelStateIsInvalid()
        {
            var sut = new EventController(null, null, null);
            sut.AddModelStateError();
            var result = await sut.MessageAllVolunteers(It.IsAny<MessageEventVolunteersViewModel>());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task MessageAllVolunteersSendsEventDetailQueryWithCorrectEventId()
        {
            const int eventId = 100;
            const int orgId = 1;
            MessageEventVolunteersViewModel viewModel = new MessageEventVolunteersViewModel { EventId = eventId };

            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.SendAsync(It.Is<OrganizationIdByEventIdQuery>(q => q.EventId == eventId)))
                .ReturnsAsync(orgId);

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            await sut.MessageAllVolunteers(viewModel);

            mediator.Verify(m => m.SendAsync(It.IsAny<OrganizationIdByEventIdQuery>()), Times.Once);
            mediator.Verify(m => m.SendAsync(It.Is<OrganizationIdByEventIdQuery>(q => q.EventId == eventId)));
        }

        [Fact]
        public async Task MessageAllVolunteersReturnsHttpUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.SendAsync(It.IsAny<OrganizationIdByEventIdQuery>()))
                .ReturnsAsync(It.IsAny<int>());

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();

            var result = await sut.MessageAllVolunteers(new MessageEventVolunteersViewModel { EventId = 100 });

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task MessageAllVolunteersSendsMessageEventVolunteersCommandWithCorrectData()
        {
            const int eventId = 100;
            const int orgId = 1;
            MessageEventVolunteersViewModel viewModel = new MessageEventVolunteersViewModel { EventId = eventId };

            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.SendAsync(It.Is<OrganizationIdByEventIdQuery>(q => q.EventId == eventId)))
                .ReturnsAsync(orgId);
            mediator
                .Setup(x => x.SendAsync(It.IsAny<MessageEventVolunteersCommand>()))
                .ReturnsAsync(It.IsAny<Unit>());

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            await sut.MessageAllVolunteers(viewModel);

            mediator.Verify(m => m.SendAsync(It.IsAny<MessageEventVolunteersCommand>()), Times.Once);
            mediator.Verify(m => m.SendAsync(It.Is<MessageEventVolunteersCommand>(c => c.ViewModel == viewModel)));
        }

        [Fact]
        public async Task MessageAllVolunteersReturnsHttpOkResult()
        {
            const int eventId = 100;
            const int orgId = 1;
            MessageEventVolunteersViewModel viewModel = new MessageEventVolunteersViewModel { EventId = eventId };

            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.SendAsync(It.Is<OrganizationIdByEventIdQuery>(q => q.EventId == eventId)))
                .ReturnsAsync(orgId);
            mediator
                .Setup(x => x.SendAsync(It.IsAny<MessageEventVolunteersCommand>()))
                .ReturnsAsync(It.IsAny<Unit>());

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.MessageAllVolunteers(viewModel);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void MessageAllVolunteersHasHttpPostAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.MessageAllVolunteers(It.IsAny<MessageEventVolunteersViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void MessageAllVolunteersHasValidateAntiForgeryTokenAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var routeAttribute = sut.GetAttributesOn(x => x.MessageAllVolunteers(It.IsAny<MessageEventVolunteersViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact]
        public async Task PostEventFileSendsEventByEventIdQueryWithCorrectEventId()
        {
            const int eventId = 99;
            const int orgId = 10;
            IFormFile formFile = null;
            string imageUrl = "url";

            var mediator = new Mock<IMediator>();
            mediator
                 .Setup(x => x.SendAsync(It.Is<OrganizationIdByEventIdQuery>(q => q.EventId == eventId)))
                 .ReturnsAsync(orgId);

            var mockImageService = new Mock<IImageService>();
            mockImageService
                .Setup(x => x.UploadEventImageAsync(It.Is<int>(i => i == orgId), It.Is<int>(i => i == eventId), It.Is<IFormFile>(f => f == formFile)))
                .ReturnsAsync(imageUrl);

            var sut = new EventController(mockImageService.Object, mediator.Object, null);

            await sut.PostEventFile(eventId, formFile);

            mediator.Verify(m => m.SendAsync(It.IsAny<OrganizationIdByEventIdQuery>()), Times.Once);
            mediator.Verify(m => m.SendAsync(It.Is<OrganizationIdByEventIdQuery>(q => q.EventId == eventId)));
        }

        [Fact]
        public async Task PostEventFileSendsUpdateEventAsyncWithCorrectData()
        {
            const int eventId = 99;
            const int orgId = 10;
            IFormFile formFile = null;
            string imageUrl = "url";

            var mediator = new Mock<IMediator>();
            mediator
                 .Setup(x => x.SendAsync(It.Is<OrganizationIdByEventIdQuery>(q => q.EventId == eventId)))
                 .ReturnsAsync(orgId);
            mediator
                 .Setup(x => x.SendAsync(It.Is<UpdateEventImageUrl>(u => u.EventId == eventId && u.ImageUrl == imageUrl)))
                 .ReturnsAsync(It.IsAny<Unit>());

            var mockImageService = new Mock<IImageService>();
            mockImageService
                .Setup(x => x.UploadEventImageAsync(It.Is<int>(i => i == orgId), It.Is<int>(i => i == eventId), It.Is<IFormFile>(f => f == formFile)))
                .ReturnsAsync(imageUrl);

            var sut = new EventController(mockImageService.Object, mediator.Object, null);

            await sut.PostEventFile(eventId, formFile);

            mediator.Verify(m => m.SendAsync(It.IsAny<UpdateEventImageUrl>()), Times.Once);
            mediator.Verify(m => m.SendAsync(It.Is<UpdateEventImageUrl>(u => u.EventId == eventId && u.ImageUrl == imageUrl)));
        }

        [Fact]
        public async Task PostEventFileRedirectsToCorrectRoute()
        {
            const int eventId = 99;
            const int orgId = 10;
            IFormFile formFile = null;
            string imageUrl = "url";

            var mediator = new Mock<IMediator>();
            mediator
                 .Setup(x => x.SendAsync(It.Is<OrganizationIdByEventIdQuery>(q => q.EventId == eventId)))
                 .ReturnsAsync(orgId);
            mediator
                 .Setup(x => x.SendAsync(It.Is<UpdateEventImageUrl>(u => u.EventId == eventId && u.ImageUrl == imageUrl)))
                 .ReturnsAsync(It.IsAny<Unit>());

            var mockImageService = new Mock<IImageService>();
            mockImageService
                .Setup(x => x.UploadEventImageAsync(It.Is<int>(i => i == orgId), It.Is<int>(i => i == eventId), It.Is<IFormFile>(f => f == formFile)))
                .ReturnsAsync(imageUrl);

            var sut = new EventController(mockImageService.Object, mediator.Object, null);

            var result = await sut.PostEventFile(eventId, formFile) as RedirectToRouteResult;

            Assert.Equal(result.RouteValues["controller"], "Event");
            Assert.Equal(result.RouteValues["Area"], "Admin");
            Assert.Equal(result.RouteValues["action"], nameof(EventController.Edit));
            Assert.Equal(result.RouteValues["id"], eventId);
        }

        [Fact]
        public void PostEventFileHasHttpPostAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.PostEventFile(It.IsAny<int>(), It.IsAny<IFormFile>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void PostEventFileHasValidateAntiForgeryTokenAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var routeAttribute = sut.GetAttributesOn(x => x.PostEventFile(It.IsAny<int>(), It.IsAny<IFormFile>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact]
        public void ControllerHasAreaAtttributeWithTheCorrectAreaName()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributes().OfType<AreaAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.RouteValue, "Admin");
        }

        [Fact]
        public void ControllerHasAreaAuthorizeAttributeWithCorrectPolicy()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Policy, "OrgAdmin");
        }

        [Fact]
        public void RequestsHasHttpGetAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Requests(It.IsAny<int>(), null)).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RequestsHasRouteAttributeWithCorrectRoute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var routeAttribute = sut.GetAttributesOn(x => x.Requests(It.IsAny<int>(), null)).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Event/[action]/{id}/{status?}");
        }

        [Fact]
        public async Task RequestsSendsOrganizationIdByEventIdQueryWithTheCorrectEventId()
        {
            const int eventId = 1;
            const int organizationId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationIdByEventIdQuery>())).ReturnsAsync(It.IsAny<int>());

            var sut = new EventController(null, mockMediator.Object, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());
            await sut.Requests(eventId, null);

            mockMediator.Verify(x => x.SendAsync(It.Is<OrganizationIdByEventIdQuery>(y => y.EventId == eventId)), Times.Once);
        }

        [Fact]
        public async Task RequestsReturnsHttpUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationIdByEventIdQuery>())).ReturnsAsync(It.IsAny<int>());

            var sut = new EventController(null, mockMediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.Requests(It.IsAny<int>(), null));
        }

        [Fact]
        public async Task RequestsReturnsRedirect_WhenStatusDoesNotMatchEnumOptions()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationIdByEventIdQuery>())).ReturnsAsync(orgId);

            var sut = new EventController(null, mockMediator.Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            Assert.IsType<RedirectToActionResult>(await sut.Requests(It.IsAny<int>(), "MadeUp"));
        }

        [Fact]
        public async Task RequestsSendsEventRequestsQueryWithCorrectEventId_WhenNoStatusRouteParamPassed_AndUserIsOrgAdmin()
        {
            const int orgId = 1;
            const int eventId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationIdByEventIdQuery>())).ReturnsAsync(orgId);
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventRequestsQuery>())).ReturnsAsync(new EventRequestsViewModel());

            var sut = new EventController(null, mockMediator.Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());
            
            await sut.Requests(eventId, null);

            mockMediator.Verify(x => x.SendAsync(It.Is<EventRequestsQuery>(y => y.EventId == eventId)), Times.Once);
        }

        [Fact]
        public async Task RequestsSendsRequestListItemsQueryWithCorrectCriteria_WhenNoStatusRouteParamPassed_AndUserIsOrgAdmin()
        {
            const int orgId = 1;
            const int eventId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationIdByEventIdQuery>())).ReturnsAsync(orgId);
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventRequestsQuery>())).ReturnsAsync(new EventRequestsViewModel());

            var sut = new EventController(null, mockMediator.Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            await sut.Requests(eventId, null);

            mockMediator.Verify(x => x.SendAsync(It.Is<RequestListItemsQuery>(y => y.Criteria.EventId == eventId && y.Criteria.Status == null)), Times.Once);
        }

        [Fact]
        public async Task RequestsSetsCorrectPageTitleOnModel_WhenStatusParamIsNotSet()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationIdByEventIdQuery>())).ReturnsAsync(orgId);
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventRequestsQuery>())).ReturnsAsync(new EventRequestsViewModel());

            var sut = new EventController(null, mockMediator.Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.Requests(1, null) as ViewResult;
            result.ShouldNotBeNull();

            var viewModel = result.Model as EventRequestsViewModel;
            viewModel.ShouldNotBeNull();

            viewModel.PageTitle.ShouldBe("All Requests");
        }

        [Fact]
        public async Task RequestsSetsCorrectPageTitleOnModel_WhenStatusParamIsSet()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationIdByEventIdQuery>())).ReturnsAsync(orgId);
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventRequestsQuery>())).ReturnsAsync(new EventRequestsViewModel());

            var sut = new EventController(null, mockMediator.Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.Requests(1, "Assigned") as ViewResult;
            result.ShouldNotBeNull();

            var viewModel = result.Model as EventRequestsViewModel;
            viewModel.ShouldNotBeNull();

            viewModel.PageTitle.ShouldBe("Assigned Requests");
        }

        private static EventController EventControllerWithNoInjectedDependencies()
        {
            return new EventController(null, null, null);
        }
    }
}