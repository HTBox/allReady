using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Security;
using AllReady.Services;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AllReady.Constants;
using Xunit;
using DeleteViewModel = AllReady.Areas.Admin.Features.Events.DeleteViewModel;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class EventAdminControllerTests
    {
        private static readonly Task TaskCompletedTask = Task.CompletedTask;

        [Fact]
        public async Task DetailsReturnsHttpNotFoundResult_WhenEventIsNull()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null, Mock.Of<IUserAuthorizationService>());
            var result = await sut.Details(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailsReturnsNotFoundResult_WhenEventIsNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync((EventDetailViewModel)null);

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());

            Assert.IsType<NotFoundResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsForbidResult_WhenEventIsNotNull_AndUserIsNotAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync(new EventDetailViewModel { Id = 1, Name = "Itinerary", OrganizationId = 1 });
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, false, false, false));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());

            Assert.IsType<ForbidResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsCorrectViewModel_WhenEventIsNotNull_AndUserAuthorized()
        {
            const int orgId = 1;
            const int eventID = 1;
            var viewModel = new EventDetailViewModel { Id = eventID, Name = "Itinerary", OrganizationId = orgId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync(viewModel);
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, false, false, false));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Details(eventID) as ViewResult;
            Assert.Null(result.ViewName);

            var resultViewModel = result.ViewData.Model;
            Assert.IsType<EventDetailViewModel>(resultViewModel);
            Assert.Equal(resultViewModel, viewModel);
        }

        [Fact]
        public async Task DetailsReturnsViewModelWithShowDeleteButton_WhenEventIsNotNull_AndUserAuthorizedToDelete()
        {
            const int orgId = 1;
            const int eventID = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync(new EventDetailViewModel { Id = eventID, Name = "Itinerary", OrganizationId = orgId });
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, true, true, true));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Details(eventID) as ViewResult;
            Assert.Null(result.ViewName);

            var resultViewModel = result.ViewData.Model as EventDetailViewModel;

            resultViewModel.ShowDeleteButton.ShouldBeTrue();
        }

        [Fact]
        public async Task DetailsReturnsViewModelWithoutShowDeleteButton_WhenEventIsNotNull_AndUserNotAuthorizedToDeleteEvent()
        {
            const int orgId = 1;
            const int eventID = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync(new EventDetailViewModel { Id = eventID, Name = "Itinerary", OrganizationId = orgId });
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, true, false, false));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Details(eventID) as ViewResult;
            Assert.Null(result.ViewName);

            var resultViewModel = result.ViewData.Model as EventDetailViewModel;

            resultViewModel.ShowCreateChildObjectButtons.ShouldBeFalse();
        }

        [Fact]
        public async Task DetailsReturnsViewModelWithShowCreateChildObjects_WhenEventIsNotNull_AndUserAuthorizedToManageChildObjects()
        {
            const int orgId = 1;
            const int eventID = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync(new EventDetailViewModel { Id = eventID, Name = "Itinerary", OrganizationId = orgId });
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, true, true, true));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Details(eventID) as ViewResult;
            Assert.Null(result.ViewName);

            var resultViewModel = result.ViewData.Model as EventDetailViewModel;

            resultViewModel.ShowCreateChildObjectButtons.ShouldBeTrue();
        }


        [Fact]
        public async Task DetailsReturnsViewModelWithoutShowCreateChildObjects_WhenEventIsNotNull_AndUserNotAuthorizedToManageChildObjects()
        {
            const int orgId = 1;
            const int eventID = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync(new EventDetailViewModel { Id = eventID, Name = "Itinerary", OrganizationId = orgId });
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, true, true, false));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Details(eventID) as ViewResult;
            Assert.Null(result.ViewName);

            var resultViewModel = result.ViewData.Model as EventDetailViewModel;

            resultViewModel.ShowCreateChildObjectButtons.ShouldBeFalse();
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
            Assert.Equal("Admin/Event/Details/{id}", routeAttribute.Template);
        }

        [Fact(Skip = "NotImplemented")]
        public async Task CreateGetSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact]
        public async Task CreateGetReturnsForbidResult_WhenUserIsNotAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel());
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, false));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());

            Assert.IsType<ForbidResult>(await sut.Create(It.IsAny<int>()));
        }

        [Fact]
        public async Task CreateGetReturnsCorrectView_AndCorrectStartAndEndDateOnViewModel()
        {
            var dateTimeTodayDate = DateTime.Today.Date;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel());
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, true));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>()) { DateTimeTodayDate = () => dateTimeTodayDate };

            var view = await sut.Create(It.IsAny<int>()) as ViewResult;
            var viewModel = view.ViewData.Model as EventEditViewModel;

            Assert.Equal("Edit", view.ViewName);
            Assert.Equal(viewModel.StartDateTime, dateTimeTodayDate);
            Assert.Equal(viewModel.EndDateTime, dateTimeTodayDate);
        }

        [Fact]
        public void CreateGetHasRouteAttributeWithCorrectRoute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var routeAttribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal("Admin/Event/Create/{campaignId}", routeAttribute.Template);
        }

        [Fact]
        public async Task CreatePostReturnsEditView_When_ModelStateNotValid()
        {
            var imageService = new Mock<IImageService>();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel());
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, true));

            var eventDetailModelValidator = new Mock<IValidateEventEditViewModels>();
            eventDetailModelValidator.Setup(x => x.Validate(It.IsAny<EventEditViewModel>(), It.IsAny<CampaignSummaryViewModel>()))
                .Returns(new List<KeyValuePair<string, string>>());

            var sut = new EventController(imageService.Object, mediator.Object, eventDetailModelValidator.Object, Mock.Of<IUserAuthorizationService>());

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
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, true));

            var eventDetailModelValidator = new Mock<IValidateEventEditViewModels>();
            eventDetailModelValidator.Setup(x => x.Validate(It.IsAny<EventEditViewModel>(), It.IsAny<CampaignSummaryViewModel>()))
                .Returns(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("ErrorKey", "ErrorMessage") });

            var sut = new EventController(imageService.Object, mediator.Object, eventDetailModelValidator.Object, Mock.Of<IUserAuthorizationService>());

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
            var sut = new EventController(null, Mock.Of<IMediator>(), null, Mock.Of<IUserAuthorizationService>());
            var routeAttribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>(), It.IsAny<EventEditViewModel>(), It.IsAny<IFormFile>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact]
        public void CreatePostHasRouteAttrbiuteWithCorrectRoute()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null, Mock.Of<IUserAuthorizationService>());
            var routeAttribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>(), It.IsAny<EventEditViewModel>(), It.IsAny<IFormFile>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal("Admin/Event/Create/{campaignId}", routeAttribute.Template);
        }

        [Fact(Skip = "NotImplemented")]
        public async void EditGetSendsEventDetailQueryWithCorrectEventId()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact]
        public async void EditGetReturnsHttpNotFoundResult_WhenEventIsNull()
        {
            var mediator = new Mock<IMediator>();
            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());
            var result = await sut.Edit(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void EditGetReturnsForbidResult_WhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync(new EventEditViewModel { Id = 1, Name = "Itinerary", OrganizationId = 1 });
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, false, false, false));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());
            sut.MakeUserNotAnOrgAdmin();
            Assert.IsType<ForbidResult>(await sut.Edit(It.IsAny<int>()));
        }

        [Fact(Skip = "NotImplemented")]
        public async void EditGetReturnsCorrectViewModel()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact]
        public async Task EditPostReturnsBadRequestResult_WhenEventIsNull()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null, Mock.Of<IUserAuthorizationService>());
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

        [Fact]
        public async Task DeleteGetReturnsNotFoundResult_WhenEventNotFound()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteQuery>()))
                .ReturnsAsync((DeleteViewModel)null);
                        mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, false, true, false));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());


            Assert.IsType<NotFoundResult>(await sut.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteGetReturnsForbidResult_WhenUserIsNotAuthorizedToDelete()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteQuery>())).ReturnsAsync(new DeleteViewModel());
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, false, false, false));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());

            Assert.IsType<ForbidResult>(await sut.Delete(It.IsAny<int>()));
        }
        
        [Fact]
        public async Task DeleteGetSendsDeleteQuery_WithCorrectEventId_WhenUserIsAuthorizedToDelete()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteQuery>()))
                .ReturnsAsync(new DeleteViewModel()).Verifiable();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, false, true, false));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());

            await sut.Delete(1);

            mediator.Verify(x => x.SendAsync(It.Is<DeleteQuery>(e => e.EventId == 1)), Times.Once);
        }

        [Fact]
        public async Task DeleteGetReturnsCorrectViewModel_WhenUserAuthorizedToDelete()
        {
            const int organizationId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteQuery>()))
                .ReturnsAsync(new DeleteViewModel { OrganizationId = organizationId });
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, false, true, false));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());

            var result = (ViewResult)await sut.Delete(It.IsAny<int>());
            var resultModel = result.ViewData.Model;

            Assert.IsType<DeleteViewModel>(resultModel);
        }    

        [Fact]
        public async Task DeleteConfirmedReturnsForbidResult_WhenUserIsNotAuthorizedToDelete()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, false, false, false));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());

            Assert.IsType<ForbidResult>(await sut.DeleteConfirmed(1));
        }

        [Fact]
        public async Task DeleteConfirmed_SendsDeleteEventCommand_WithCorrectEventId_WhenUserIsAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, false, true, false));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());

            await sut.DeleteConfirmed(1);

            mediator.Verify(x => x.SendAsync(It.Is<DeleteEventCommand>(e => e.EventId == 1)), Times.Once);
        }

        [Fact]
        public async Task DeleteConfirmed_RedirectsToCorrectAction_AndControllerWithCorrectRouteValues_AfterSendingDeleteCommand()
        {
            const int cmapaignId = 20;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, false, true, false, cmapaignId));

            var sut = new EventController(null, mediator.Object, null, Mock.Of<IUserAuthorizationService>());

            var result = await sut.DeleteConfirmed(1);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            redirectResult.ControllerName.ShouldBe("Campaign");
            redirectResult.ActionName.ShouldBe("Details");

            redirectResult.RouteValues["area"].ShouldBe(AreaNames.Admin);
            redirectResult.RouteValues["id"].ShouldBe(cmapaignId);
        }

        [Fact]
        public void DeleteConfirmed_HasHttpPostAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<int>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DeleteConfirmed_HasActionNameAttribute_WithCorrectName()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<int>())).OfType<ActionNameAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("Delete", attribute.Name);
        }

        [Fact]
        public void DeleteConfirmed_HasValidateAntiForgeryTokenAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var routeAttribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<int>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact]
        public async Task DeleteEventImage_ReturnsJsonObjectWithStatusOfNotFound()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync((EventEditViewModel)null);
            var imageServiceMock = new Mock<IImageService>();
            var eventEditViewModelValidatorMock = new Mock<IValidateEventEditViewModels>();
            var sut = new EventController(imageServiceMock.Object, mediatorMock.Object, eventEditViewModelValidatorMock.Object, Mock.Of<IUserAuthorizationService>());

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
        public async Task DeleteEventImage_ReturnsJsonObjectWithStatusOfUnauthorizedIfUserIsNotAuthorized()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync(new EventEditViewModel());
            mediatorMock.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, false, false, false));

            var imageServiceMock = new Mock<IImageService>();
            var eventEditViewModelValidatorMock = new Mock<IValidateEventEditViewModels>();
            var sut = new EventController(imageServiceMock.Object, mediatorMock.Object, eventEditViewModelValidatorMock.Object, Mock.Of<IUserAuthorizationService>());
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
            var sut = new EventController(imageServiceMock.Object, mediatorMock.Object, eventEditViewModelValidatorMock.Object, Mock.Of<IUserAuthorizationService>());

            const int eventId = 2;

            await sut.DeleteEventImage(eventId);

            mediatorMock.Verify(m => m.SendAsync(It.IsAny<EventEditQuery>()), Times.Once);
            mediatorMock.Verify(m => m.SendAsync(It.Is<EventEditQuery>(s => s.EventId == eventId)));
        }


        [Fact]
        public async Task DeleteEventImage_CallsDeleteImageAsyncWithCorrectData()
        {
            var eventEditViewModel = new EventEditViewModel
            {
                OrganizationId = 1,
                ImageUrl = "URL!"
            };

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync(eventEditViewModel);
            mediatorMock.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, true, false, false));

            var imageServiceMock = new Mock<IImageService>();
            var eventEditViewModelValidatorMock = new Mock<IValidateEventEditViewModels>();
            var sut = new EventController(imageServiceMock.Object, mediatorMock.Object, eventEditViewModelValidatorMock.Object, Mock.Of<IUserAuthorizationService>());
            sut.MakeUserAnOrgAdmin(eventEditViewModel.OrganizationId.ToString());

            await sut.DeleteEventImage(It.IsAny<int>());

            imageServiceMock.Verify(i => i.DeleteImageAsync(It.Is<string>(f => f == "URL!")), Times.Once);
        }


        [Fact]
        public async Task DeleteEventImage_CallsEditEventCommandWithCorrectData()
        {
            var eventEditViewModel = new EventEditViewModel
            {
                OrganizationId = 1,
                ImageUrl = "URL!"
            };

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync(eventEditViewModel);
            mediatorMock.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, true, false, false));

            var imageServiceMock = new Mock<IImageService>();
            var eventEditViewModelValidatorMock = new Mock<IValidateEventEditViewModels>();
            var sut = new EventController(imageServiceMock.Object, mediatorMock.Object, eventEditViewModelValidatorMock.Object, Mock.Of<IUserAuthorizationService>());
            sut.MakeUserAnOrgAdmin(eventEditViewModel.OrganizationId.ToString());

            await sut.DeleteEventImage(It.IsAny<int>());

            mediatorMock.Verify(m => m.SendAsync(It.Is<EditEventCommand>(s => s.Event == eventEditViewModel)), Times.Once);
        }


        [Fact]
        public async Task DeleteEventImage_ReturnsJsonObjectWithStatusOfSuccessIfImageDeletedSuccessfully()
        {
            var eventEditViewModel = new EventEditViewModel
            {
                OrganizationId = 1,
                ImageUrl = "URL!"
            };

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync(eventEditViewModel);
            mediatorMock.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, true, false, false));

            var imageServiceMock = new Mock<IImageService>();
            var eventEditViewModelValidatorMock = new Mock<IValidateEventEditViewModels>();
            var sut = new EventController(imageServiceMock.Object, mediatorMock.Object, eventEditViewModelValidatorMock.Object, Mock.Of<IUserAuthorizationService>());
            sut.MakeUserAnOrgAdmin(eventEditViewModel.OrganizationId.ToString());

            var result = await sut.DeleteEventImage(It.IsAny<int>());

            result.Value.GetType()
                .GetTypeInfo()
                .GetProperty("status")
                .GetValue(result.Value)
                .ShouldBe("Success");
        }


        [Fact]
        public async Task DeleteEventImage_ReturnsJsonObjectWithStatusOfNothingToDeleteIfThereWasNoExistingImage()
        {
            var eventEditViewModel = new EventEditViewModel
            {
                OrganizationId = 1
            };

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync(eventEditViewModel);
            mediatorMock.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, true, false, false));

            var imageServiceMock = new Mock<IImageService>();
            var eventEditViewModelValidatorMock = new Mock<IValidateEventEditViewModels>();
            var sut = new EventController(imageServiceMock.Object, mediatorMock.Object, eventEditViewModelValidatorMock.Object, Mock.Of<IUserAuthorizationService>());
            sut.MakeUserAnOrgAdmin(eventEditViewModel.OrganizationId.ToString());

            var result = await sut.DeleteEventImage(It.IsAny<int>());

            result.Value.GetType()
                .GetTypeInfo()
                .GetProperty("status")
                .GetValue(result.Value)
                .ShouldBe("NothingToDelete");
        }


        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersReturnsBadRequestObjectResult_WhenModelStateIsInvalid()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersSendsEventDetailQueryWithCorrectEventId()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersReturnsHttpNotFoundResult_WhenEventIsNull()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersReturnsHttpUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersSendsMessageEventVolunteersCommandWithCorrectData()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersReturnsHttpOkResult()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
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

        [Fact(Skip = "NotImplemented")]
        public async Task PostEventFileSendsEventByEventIdQueryWithCorrectEventId()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task PostEventFileSendsUpdateEventAsyncWithCorrectData()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task PostEventFileRedirectsToCorrectRoute()
        {
            // delete this line when starting work on this unit test
            await TaskCompletedTask;
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
            Assert.Equal(AreaNames.Admin, attribute.RouteValue);
        }

        [Fact]
        public void ControllerHasAreaAuthorizeAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void Requests_HasHttpGetAttribute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Requests(It.IsAny<int>(), null)).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void Requests_HasRouteAttributeWithCorrectRoute()
        {
            var sut = EventControllerWithNoInjectedDependencies();
            var routeAttribute = sut.GetAttributesOn(x => x.Requests(It.IsAny<int>(), null)).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal("Admin/Event/[action]/{id}/{status?}", routeAttribute.Template);
        }

        [Fact]
        public async Task Requests_SendsEventRequestsQueryWithTheCorrectEventId()
        {
            const int eventId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, false, false, false));
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventRequestsQuery>())).ReturnsAsync(new EventRequestsViewModel());

            var sut = new EventController(null, mockMediator.Object, null, Mock.Of<IUserAuthorizationService>());
            await sut.Requests(eventId, null);

            mockMediator.Verify(x => x.SendAsync(It.Is<EventRequestsQuery>(y => y.EventId == eventId)), Times.Once);
        }

        [Fact]
        public async Task Requests_ReturnsHttpForbidResult_WhenUserIsNotAuthorized()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationIdByEventIdQuery>())).ReturnsAsync(It.IsAny<int>());
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, false, false, false));

            var sut = new EventController(null, mockMediator.Object, null, Mock.Of<IUserAuthorizationService>());

            Assert.IsType<ForbidResult>(await sut.Requests(It.IsAny<int>(), null));
        }

        [Fact]
        public async Task Requests_ReturnsRedirect_WhenStatusDoesNotMatchEnumOptions()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, false, false, false));

            var sut = new EventController(null, mockMediator.Object, null, Mock.Of<IUserAuthorizationService>());

            Assert.IsType<RedirectToActionResult>(await sut.Requests(It.IsAny<int>(), "MadeUp"));
        }

        [Fact]
        public async Task Requests_SendsEventRequestsQueryWithCorrectEventId_WhenNoStatusRouteParamPassed_AndUserIsOrgAdmin()
        {
            const int eventId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, false, false, false));
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventRequestsQuery>())).ReturnsAsync(new EventRequestsViewModel());

            var sut = new EventController(null, mockMediator.Object, null, Mock.Of<IUserAuthorizationService>());

            await sut.Requests(eventId, null);

            mockMediator.Verify(x => x.SendAsync(It.Is<EventRequestsQuery>(y => y.EventId == eventId)), Times.Once);
        }

        [Fact]
        public async Task Requests_SendsRequestListItemsQueryWithCorrectCriteria_WhenNoStatusRouteParamPassed_AndUserAuthorized()
        {
            const int eventId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, false, false, false));
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventRequestsQuery>())).ReturnsAsync(new EventRequestsViewModel());

            var sut = new EventController(null, mockMediator.Object, null, Mock.Of<IUserAuthorizationService>());

            await sut.Requests(eventId, null);

            mockMediator.Verify(x => x.SendAsync(It.Is<RequestListItemsQuery>(y => y.Criteria.EventId == eventId && y.Criteria.Status == null)), Times.Once);
        }

        [Fact]
        public async Task Requests_SetsCorrectPageTitleOnModel_WhenStatusParamIsNotSet()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, false, false, false));
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventRequestsQuery>())).ReturnsAsync(new EventRequestsViewModel());

            var sut = new EventController(null, mockMediator.Object, null, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Requests(1, null) as ViewResult;
            result.ShouldNotBeNull();

            var viewModel = result.Model as EventRequestsViewModel;
            viewModel.ShouldNotBeNull();

            viewModel.PageTitle.ShouldBe("All Requests");
        }

        [Fact]
        public async Task Requests_SetsCorrectPageTitleOnModel_WhenStatusParamIsSet()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(true, false, false, false));
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventRequestsQuery>())).ReturnsAsync(new EventRequestsViewModel());

            var sut = new EventController(null, mockMediator.Object, null, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Requests(1, "Assigned") as ViewResult;
            result.ShouldNotBeNull();

            var viewModel = result.Model as EventRequestsViewModel;
            viewModel.ShouldNotBeNull();

            viewModel.PageTitle.ShouldBe("Assigned Requests");
        }

        private static EventController EventControllerWithNoInjectedDependencies()
        {
            return new EventController(null, null, null, null);
        }
    }
}
