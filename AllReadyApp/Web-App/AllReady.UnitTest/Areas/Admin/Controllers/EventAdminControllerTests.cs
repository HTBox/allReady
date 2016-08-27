﻿using System;
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
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Features.Event;
using AllReady.Areas.Admin.ViewModels.Campaign;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using AllReady.ViewModels.Event;
using Shouldly;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class EventAdminControllerTests
    {
        //delete this line when all unit tests using it have been completed
        private static readonly Task<int> TaskFromResultZero = Task.FromResult(0);

        [Fact(Skip = "NotImplemented")]
        public async Task DetailsSendsEventDetailQueryAsyncWithCorrectEventId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundResultWhenEventIsNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel());

            var sut = new EventController(null, mediator.Object, null);
            var result = await sut.Details(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailsReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync(new EventDetailViewModel { Id = 1, Name = "Itinerary", OrganizationId = 1 });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();
            Assert.IsType<UnauthorizedResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsHttpUnauthorizedResultWhenEventIsNotNullAndUserIsNotAnOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync(new EventDetailViewModel { Id = 1, Name = "Itinerary", OrganizationId = 1 });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();
            Assert.IsType<UnauthorizedResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DetailsReturnsCorrectViewModelWhenEventIsNotNullAndUserIsOrgAdmin()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void DetailsHasHttpGetAttribute()
        {
            var sut = new EventController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DetailsHasRouteAttributeWithCorrectRoute()
        {
            var sut = new EventController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Event/Details/{id}");
        }

        [Fact(Skip = "NotImplemented")]
        public async Task CreateGetSendsCampaignSummaryQueryAsyncWithCorrectCampaignId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task CreateGetReturnsHttpUnauthorizedResultWhenCampaignIsNull()
        {
            var mediator = new Mock<IMediator>();
            var sut = new EventController(null, mediator.Object, null);

            Assert.IsType<UnauthorizedResult>(await sut.Create(It.IsAny<int>()));
        }

        [Fact]
        public async Task CreateGetReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync(new EventDetailViewModel { Id = 1, Name = "Itinerary", OrganizationId = 1 });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.Create(It.IsAny<int>()));
        }

        [Fact(Skip = "NotImplemented")]
        public async Task CreateGetReturnsCorrectViewAndViewModel()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void CreateGetHasRouteAttributeWithCorrectRoute()
        {
            var sut = new EventController(null, null, null);
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

            var eventDetailModelValidator = new Mock<IValidateEventDetailModels>();
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

            var eventDetailModelValidator = new Mock<IValidateEventDetailModels>();
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
            var sut = new EventController(null, null, null);
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

        [Fact(Skip = "NotImplemented")]
        public async void EditGetSendsEventDetailQueryAsyncWithCorrectEventId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async void EditGetReturnsHttpNotFoundResultWhenEventIsNull()
        {
            var mediator = new Mock<IMediator>();
            var sut = new EventController(null, mediator.Object, null);
            var result = await sut.Edit(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void EditGetReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventEditQuery>())).ReturnsAsync(new EventEditViewModel { Id = 1, Name = "Itinerary", OrganizationId = 1 });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();
            Assert.IsType<UnauthorizedResult>(await sut.Edit(It.IsAny<int>()));
        }

        [Fact(Skip = "NotImplemented")]
        public async void EditGetReturnsCorrectViewModel()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task EditPostReturnsBadRequestResultWhenEventIsNull()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null);
            Assert.IsType<BadRequestResult>(await sut.Edit(It.IsAny<EventEditViewModel>(), It.IsAny<IFormFile>()).ConfigureAwait(false));
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostSendsManagingOrganizationIdByEventIdQueryWithCorrectEventId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdminUser()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostSendsCampaignSummaryQueryAsyncWithTheCorrectCampaignId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostAddsValidationErrorsToModelStateErrorsWhenEventDetailsModelValidatorHasErrors()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostReturnsCorrectViewWhenEventDetailsModelValidatorHasErrors()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostInvokesUploadEventImageAsyncWithTheCorrectParametersWhenModelStateIsValidAndFileUploadIsNotNullAndFileUploadIsAnAcceptableImageContentType()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostAddsCorrectKeyAndValueToModelStateErrorsWhenModelStateIsValidAndFileUploadIsNotNullAndFileUploadIsNotAnAcceptableImageContentType()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostReturnsCorrectViewModelWhenModelStateIsValidAndFileUploadIsNotNullAndFileUploadIsNotAnAcceptableImageContentType()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostSendsEditEventCommandAsyncWithCorrectEventWhenModelStateIsValid()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostRedirectsToCorrectActionAndControllerWithCorrectRouteValuesWhenModelStateIsValid()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostReturnsCorrectViewModelWhenModelStateIsNotValid()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void EditPostHasHttpPostAttribute()
        {
            var sut = new EventController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<EventEditViewModel>(), It.IsAny<IFormFile>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void EditPostHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null);
            var routeAttribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<EventEditViewModel>(), It.IsAny<IFormFile>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteSendsEventDetailQueryAsyncWithCorrectEventId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task DeleteReturnsHttpNotFoundResultWhenEventIsNull()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null);
            Assert.IsType<NotFoundResult>(await sut.Delete(It.IsAny<int>()).ConfigureAwait(false));
        }

        [Fact]
        public async Task DeleteReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync(new EventDetailViewModel { Id = 1, Name = "Itinerary", OrganizationId = 1 });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.Delete(It.IsAny<int>()).ConfigureAwait(false));
        }

        [Fact]
        public async Task DeleteReturnsCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync(new EventDetailViewModel { Id = 1, Name = "Itinerary", OrganizationId = 1 });

            var sut = new EventController(null, mediator.Object, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, It.IsAny<int>().ToString())
            });

            var result = (ViewResult) await sut.Delete(It.IsAny<int>()).ConfigureAwait(false);
            var resultModel = result.ViewData.Model;

            Assert.IsType<EventDetailViewModel>(resultModel);
        }

        [Fact]
        public void DeleteHasActionNameAttributeWithCorrectName()
        {
            ActionNameAttribute attr = (ActionNameAttribute)typeof(EventController).GetMethod(nameof(EventController.Delete), new Type[] { typeof(int) }).GetCustomAttribute(typeof(ActionNameAttribute));
            Assert.Equal(attr.Name, "Delete");
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteConfirmedSendsEventDetailQueryAsyncWithCorrectEventId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task DeleteConfirmedReturnsHttpNotFoundResultWhenEventIsNull()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null);
            Assert.IsType<NotFoundResult>(await sut.DeleteConfirmed(It.IsAny<int>()).ConfigureAwait(false));
        }

        [Fact]
        public async Task DeleteConfirmedReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventDetailQuery>())).ReturnsAsync(new EventDetailViewModel { Id = 1, Name = "Itinerary", OrganizationId = 1 });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.DeleteConfirmed(It.IsAny<int>()).ConfigureAwait(false));
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteConfirmedSendsDeleteEventCommandAsyncWithCorrectEventId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteConfirmedRedirectToCorrectActionAndControllerWithCorrectRouteValues()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void DeleteConfirmedHasHttpPostAttribute()
        {
            var sut = new EventController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<int>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DeleteConfirmedHasActionNameAttributeWithCorrectName()
        {
            ActionNameAttribute attr = (ActionNameAttribute)typeof(EventController).GetMethod(nameof(EventController.DeleteConfirmed), new Type[] { typeof(int) }).GetCustomAttribute(typeof(ActionNameAttribute));
            Assert.Equal(attr.Name, "Delete");
        }

        [Fact]
        public void DeleteConfirmedHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null);
            var routeAttribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<int>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignSendsEventByEventIdQueryWithCorrectEventId()
        {
        }

        [Fact]
        public void AssignReturnsHttpNotFoundResultWhenEventIsNull()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null);
            Assert.IsType<NotFoundResult>(sut.Assign(It.IsAny<int>()));
        }

        [Fact]
        public void AssignReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { CampaignId = 1, Campaign = new Campaign { ManagingOrganizationId = 1 } });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(sut.Assign(It.IsAny<int>()));
        }

        [Fact]
        public void AssignReturnsCorrectViewModel()
        {
            const int orgId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { CampaignId = 1, Campaign = new Campaign { ManagingOrganizationId = orgId } });

            var sut = new EventController(null, mediator.Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = (ViewResult) sut.Assign(It.IsAny<int>());
            var resultModel = result.ViewData.Model;

            Assert.IsType<EventViewModel>(resultModel);
        }

        [Fact]
        public void AssignHasHttpGetAttribute()
        {
            var sut = new EventController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Assign(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersReturnsBadRequestObjectResultWhenModelStateIsInvalid()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersSendsEventDetailQueryAsyncWithCorrectEventId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersReturnsHttpNotFoundResultWhenEventIsNull()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersSendsMessageEventVolunteersCommandAsyncWithCorrectData()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersReturnsHttpOkResult()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void MessageAllVolunteersHasHttpPostAttribute()
        {
            var sut = new EventController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.MessageAllVolunteers(It.IsAny<MessageEventVolunteersViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void MessageAllVolunteersHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null);
            var routeAttribute = sut.GetAttributesOn(x => x.MessageAllVolunteers(It.IsAny<MessageEventVolunteersViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact(Skip = "NotImplemented")]
        public async Task PostEventFileSendsEventByEventIdQueryWithCorrectEventId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task PostEventFileSendsUpdateEventAsyncWithCorrectData()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task PostEventFileRedirectsToCorrectRoute()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void PostEventFileHasHttpPostAttribute()
        {
            var sut = new EventController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.PostEventFile(It.IsAny<int>(), It.IsAny<IFormFile>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void PostEventFileHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new EventController(null, Mock.Of<IMediator>(), null);
            var routeAttribute = sut.GetAttributesOn(x => x.PostEventFile(It.IsAny<int>(), It.IsAny<IFormFile>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact]
        public void ControllerHasAreaAtttributeWithTheCorrectAreaName()
        {
            var sut = new EventController(null, null, null);
            var attribute = sut.GetAttributes().OfType<AreaAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.RouteValue, "Admin");
        }

        [Fact]
        public void ControllerHasAreaAuthorizeAttributeWithCorrectPolicy()
        {
            var sut = new EventController(null, null, null);
            var attribute = sut.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Policy, "OrgAdmin");
        }

        [Fact]
        public void RequestsHasHttpGetAttribute()
        {
            var sut = new EventController(Mock.Of<IImageService>(), Mock.Of<IMediator>(), Mock.Of<IValidateEventDetailModels>());
            var attribute = sut.GetAttributesOn(x => x.Requests(It.IsAny<int>(), null)).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RequestsHasRouteAttributeWithCorrectRoute()
        {
            var sut = new EventController(Mock.Of<IImageService>(), Mock.Of<IMediator>(), Mock.Of<IValidateEventDetailModels>());
            var routeAttribute = sut.GetAttributesOn(x => x.Requests(It.IsAny<int>(), null)).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Event/[action]/{id}/{status?}");
        }

        [Fact]
        public async Task RequestsSendsEventByIdQuery()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<EventByIdQuery>())).Returns(It.IsAny<Event>()).Verifiable();

            var sut = new EventController(Mock.Of<IImageService>(), mockMediator.Object, Mock.Of<IValidateEventDetailModels>());
            await sut.Requests(1, null);

            mockMediator.Verify(x => x.Send(It.IsAny<EventByIdQuery>()), Times.Once);
        }

        [Fact]
        public async Task RequestsReturnsHttpNotFoundResultWhenEventIsNull()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<EventByIdQuery>())).Returns((Event)null).Verifiable();

            var sut = new EventController(Mock.Of<IImageService>(), mockMediator.Object, Mock.Of<IValidateEventDetailModels>());

            Assert.IsType<NotFoundResult>(await sut.Requests(It.IsAny<int>(), null));
        }

        [Fact]
        public async Task RequestsReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganizationId = 1000 } }).Verifiable();
            
            var sut = new EventController(Mock.Of<IImageService>(), mockMediator.Object, Mock.Of<IValidateEventDetailModels>());
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.Requests(It.IsAny<int>(), null));
        }

        [Fact]
        public async Task RequestsReturnsRedirectWhenStatusDoesNotMatchEnumOptions()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganizationId = orgId } }).Verifiable();

            var sut = new EventController(Mock.Of<IImageService>(), mockMediator.Object, Mock.Of<IValidateEventDetailModels>());
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            Assert.IsType<RedirectToActionResult>(await sut.Requests(It.IsAny<int>(), "MadeUp"));
        }

        [Fact]
        public async Task RequestsSendsEventRequestsQueryWhenNoStatusRouteParamPassedAndUserIsOrgAdmin()
        {
            const int orgId = 1;
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganizationId = orgId } }).Verifiable();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventRequestsQuery>())).ReturnsAsync(new EventRequestsViewModel()).Verifiable();

            var sut = new EventController(Mock.Of<IImageService>(), mockMediator.Object, Mock.Of<IValidateEventDetailModels>());
            sut.MakeUserAnOrgAdmin(orgId.ToString());
            
            await sut.Requests(1, null);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<EventRequestsQuery>()), Times.Once);
        }

        [Fact]
        public async Task RequestsSendsEventRequestListItemsQueryWhenNoStatusRouteParamPassedAndUserIsOrgAdmin()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganizationId = orgId } }).Verifiable();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventRequestsQuery>())).ReturnsAsync(new EventRequestsViewModel()).Verifiable();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<RequestListItemsQuery>())).ReturnsAsync(new List<RequestListViewModel>()).Verifiable();

            var sut = new EventController(Mock.Of<IImageService>(), mockMediator.Object, Mock.Of<IValidateEventDetailModels>());
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            await sut.Requests(1, null);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<RequestListItemsQuery>()), Times.Once);
        }

        [Fact]
        public async Task RequestsSendsEventRequestsQueryWhenValidStatusRouteParamPassedAndUserIsOrgAdmin()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganizationId = orgId }}).Verifiable();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventRequestsQuery>())).ReturnsAsync(new EventRequestsViewModel()).Verifiable();

            var sut = new EventController(Mock.Of<IImageService>(), mockMediator.Object, Mock.Of<IValidateEventDetailModels>());
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            await sut.Requests(1, "Assigned");

            mockMediator.Verify(x => x.SendAsync(It.IsAny<EventRequestsQuery>()), Times.Once);
        }

        [Fact]
        public async Task RequestsSetsCorrectPageTitleOnModelWhenStatusParamIsNotSet()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganizationId = orgId } }).Verifiable();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventRequestsQuery>())).ReturnsAsync(new EventRequestsViewModel()).Verifiable();

            var sut = new EventController(Mock.Of<IImageService>(), mockMediator.Object, Mock.Of<IValidateEventDetailModels>());
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.Requests(1, null) as ViewResult;

            result.ShouldNotBeNull();

            var castModel = result.Model as EventRequestsViewModel;

            castModel.ShouldNotBeNull();

            castModel.PageTitle.ShouldBe("All Requests");
        }

        [Fact]
        public async Task RequestsSetsCorrectPageTitleOnModelWhenStatusParamIsSet()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.Send(It.IsAny<EventByIdQuery>())).Returns(new Event { Campaign = new Campaign { ManagingOrganizationId = 1 } }).Verifiable();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventRequestsQuery>())).ReturnsAsync(new EventRequestsViewModel()).Verifiable();

            var sut = new EventController(Mock.Of<IImageService>(), mockMediator.Object, Mock.Of<IValidateEventDetailModels>());
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            var result = await sut.Requests(1, "Assigned") as ViewResult;

            result.ShouldNotBeNull();

            var castModel = result.Model as EventRequestsViewModel;

            castModel.ShouldNotBeNull();

            castModel.PageTitle.ShouldBe("Assigned Requests");
        }

        private static EventController GetEventController()
        {
            return GetEventController(new DateTimeOffset(), new DateTimeOffset());
        }

        private static EventController GetEventController(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var mediator = new Mock<IMediator>();
            var imageService = new Mock<IImageService>();

            mediator.Setup(x => x.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel { StartDate = startDate, EndDate = endDate });

            var sut = new EventController(imageService.Object, mediator.Object, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            return sut;
        }
    }
}