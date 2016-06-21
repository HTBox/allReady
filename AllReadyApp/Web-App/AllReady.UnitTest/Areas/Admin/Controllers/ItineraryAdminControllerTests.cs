﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;
using System.Linq;
using AllReady.Areas.Admin.Features.Events;
using Microsoft.AspNet.Authorization;
using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Areas.Admin.Models;
using AllReady.Areas.Admin.Models.ItineraryModels;
using AllReady.Areas.Admin.Models.Validators;
using AllReady.Areas.Admin.Features.Organizations;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class ItineraryAdminControllerTests
    {
        //delete this line when all unit tests using it have been completed
        private readonly Task taskFromResultZero = Task.FromResult(0);

        [Fact]
        public void ControllerHasAreaAtttributeWithTheCorrectAreaName()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            var attribute = sut.GetAttributes().OfType<AreaAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.RouteValue, "Admin");
        }

        [Fact]
        public void ControllerHasAreaAuthorizeAttributeWithCorrectPolicy()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            var attribute = sut.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Policy, "OrgAdmin");
        }

        [Fact]
        public void DetailsHasHttpGetAttribute()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            var attribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DetailsHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            var routeAttribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/Details/{id}");
        }

        [Fact]
        public async Task DetailsSendsEventDetailQueryAsyncWithCorrectEventId()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(null).Verifiable();

            var sut = new ItineraryController(mockMediator.Object, MockSuccessValidation().Object);
            await sut.Details(1);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>()), Times.Once);
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundResultWhenEventIsNull()
        {
            ItineraryController controller;
            MockMediatorItineraryDetailQuery(out controller);
            Assert.IsType<HttpNotFoundResult>(await controller.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var sut = GetItineraryControllerWithDetailsQuery(UserType.OrgAdmin.ToString(), It.IsAny<int>());
            Assert.IsType<HttpUnauthorizedResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsCorrectViewAndViewModelWhenEventIsNotNullAndUserIsOrgAdmin()
        {
            var sut = GetItineraryControllerWithDetailsQuery(UserType.SiteAdmin.ToString(), 0);
            var result = await sut.Details(It.IsAny<int>()) as ViewResult;
            Assert.Equal(result.ViewName, "Details");

            var resultViewModel = result.ViewData.Model;
            Assert.IsType<ItineraryDetailsModel>(resultViewModel);
        }

        [Fact]
        public async Task DetailsReturnsCorrectViewModelWhenEventIsNotNullAndUserIsSiteAdmin()
        {
            var sut = GetItineraryControllerWithDetailsQuery(UserType.SiteAdmin.ToString(), 0);
            Assert.IsType<ViewResult>(await sut.Details(It.IsAny<int>()));
        }
        
        [Fact]
        public void CreateHasHttpPostAttribute()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            var attribute = sut.GetAttributesOn(x => x.Create(It.IsAny<ItineraryEditModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void CreateHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            var routeAttribute = sut.GetAttributesOn(x => x.Create(It.IsAny<ItineraryEditModel>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/Create");
        }

        [Fact]
        public void CreateHasValidateAntiForgeryAttribute()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            var routeAttribute = sut.GetAttributesOn(x => x.Create(It.IsAny<ItineraryEditModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact]
        public async Task CreateReturnsHttpBadRequestWhenModelIsNull()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            Assert.IsType<BadRequestResult>(await sut.Create(null).ConfigureAwait(false));
        }

        [Fact]
        public async Task CreateSendsEventSummaryQueryWithCorrectEventId()
        {
            var model = new ItineraryEditModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>()))
                .ReturnsAsync(It.IsAny<EventSummaryModel>()).Verifiable();

            var sut = new ItineraryController(mockMediator.Object, MockSuccessValidation().Object);
            await sut.Create(model);

            mockMediator.Verify(x => x.SendAsync(It.Is<EventSummaryQuery>(y => y.EventId == model.EventId)), Times.Once);
        }

        [Fact]
        public async Task CreateReturnsHttpBadRequestIfEventNull()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(null);

            var sut = new ItineraryController(mockMediator.Object, MockSuccessValidation().Object);
            Assert.IsType<BadRequestResult>(await sut.Create(It.IsAny<ItineraryEditModel>()).ConfigureAwait(false));
        }

        [Fact]
        public async Task CreateReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdminForEventOrg()
        {
            var model = new ItineraryEditModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var sut = GetItineraryControllerWithEventSummaryQuery(UserType.OrgAdmin.ToString(), 5);
            Assert.IsType<HttpUnauthorizedResult>(await sut.Create(model));
        }

        [Fact]
        public async Task CreateReturnsOkResultWhenUserIsOrgAdmin()
        {
            var model = new ItineraryEditModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var sut = GetItineraryControllerWithEventSummaryQuery(UserType.OrgAdmin.ToString(), 1);
            Assert.IsType<HttpOkObjectResult>(await sut.Create(model));
        }

        [Fact]
        public async Task CreateReturnsOkResultWhenUserIsSiteAdmin_AndModelIsValid_AndSuccessfulAdd()
        {
            var model = new ItineraryEditModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var sut = GetItineraryControllerWithEventSummaryQuery(UserType.SiteAdmin.ToString(), 0);
            Assert.IsType<HttpOkObjectResult>(await sut.Create(model));
        }

        [Fact]
        public async Task CreateReturnsHttpBadRequestResultWhenModelStateHasError()
        {
            var model = new ItineraryEditModel
            {
                EventId = 1,
                Date = new DateTime(2016, 06, 01)
            };

            var sut = GetItineraryControllerWithEventSummaryQuery(UserType.SiteAdmin.ToString(), 0);

            sut.ModelState.AddModelError("key", "Error");

            var result = await sut.Create(model);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateCallsValidatorWithCorrectItineraryEditModelAndEventSummaryModel()
        {
            var model = new ItineraryEditModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var mediator = new Mock<IMediator>();

            var eventSummaryModel = new EventSummaryModel
            {
                Id = 1, Name = "Event", OrganizationId = 1, StartDateTime = new DateTimeOffset(new DateTime(2016, 01, 01)), EndDateTime = new DateTimeOffset(new DateTime(2016, 12, 31))
            };
            mediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(eventSummaryModel);
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryCommand>())).ReturnsAsync(1);

            var mockValidator = new Mock<IItineraryEditModelValidator>();
            mockValidator.Setup(mock => mock.Validate(model, eventSummaryModel)).Returns(new List<KeyValuePair<string, string>>()).Verifiable();

            var sut = new ItineraryController(mediator.Object, mockValidator.Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            await sut.Create(model);

            mockValidator.Verify(x => x.Validate(model, eventSummaryModel), Times.Once);
        }

        [Fact]
        public async Task CreateReturnsHttpBadRequestResultWhenValidatonFails()
        {
            var model = new ItineraryEditModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryModel { Id = 1, Name = "Event", OrganizationId = 1, StartDateTime = new DateTimeOffset(new DateTime(2016, 01, 01)), EndDateTime = new DateTimeOffset(new DateTime(2016, 12, 31)) });
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryCommand>())).ReturnsAsync(1);

            var validatorError = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key", "value")
            };

            var mockValidator = new Mock<IItineraryEditModelValidator>();
            mockValidator.Setup(mock => mock.Validate(It.IsAny<ItineraryEditModel>(), It.IsAny<EventSummaryModel>())).Returns(validatorError).Verifiable();

            var sut = new ItineraryController(mediator.Object, mockValidator.Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            var result = await sut.Create(model);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateSendsEditItineraryCommandWithCorrectItineraryEditModel()
        {
            var model = new ItineraryEditModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryModel { Id = 1, Name = "Event", OrganizationId = 1, StartDateTime = new DateTimeOffset(new DateTime(2016, 01, 01)), EndDateTime = new DateTimeOffset(new DateTime(2016, 12, 31)) });
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryCommand>())).ReturnsAsync(0).Verifiable();

            var mockValidator = new Mock<IItineraryEditModelValidator>();
            mockValidator.Setup(mock => mock.Validate(It.IsAny<ItineraryEditModel>(), It.IsAny<EventSummaryModel>())).Returns(new List<KeyValuePair<string, string>>()).Verifiable();

            var sut = new ItineraryController(mediator.Object, mockValidator.Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            await sut.Create(model);

            mediator.Verify(x => x.SendAsync(It.Is<EditItineraryCommand>(y => y.Itinerary == model)), Times.Once);
        }

        [Fact]
        public async Task CreateReturnsHttpBadRequestResultWhenEditItineraryReturnsZero()
        {
            var model = new ItineraryEditModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryModel { Id = 1, Name = "Event", OrganizationId = 1, StartDateTime = new DateTimeOffset(new DateTime(2016, 01, 01)), EndDateTime = new DateTimeOffset(new DateTime(2016, 12, 31)) });
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryCommand>())).ReturnsAsync(0);

            var mockValidator = new Mock<IItineraryEditModelValidator>();
            mockValidator.Setup(mock => mock.Validate(It.IsAny<ItineraryEditModel>(), It.IsAny<EventSummaryModel>())).Returns(new List<KeyValuePair<string, string>>()).Verifiable();

            var sut = new ItineraryController(mediator.Object, mockValidator.Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            var result = await sut.Create(model);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void AddTeamMemberHasHttpPostAttribute()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            var attribute = sut.GetAttributesOn(x => x.AddTeamMember(It.IsAny<int>(), It.IsAny<int>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void AddTeamMemberHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            var routeAttribute = sut.GetAttributesOn(x => x.AddTeamMember(It.IsAny<int>(), It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/AddTeamMember");
        }

        [Fact]
        public void AddTeamMemberHasValidateAntiForgeryAttribute()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            var routeAttribute = sut.GetAttributesOn(x => x.AddTeamMember(It.IsAny<int>(), It.IsAny<int>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact]
        public async Task AddTeamMemberSendsAddTeamMemberCommandWithCorrectParameters()
        {
            const int itineraryId = 1;
            const int selectedTeamMember = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AddTeamMemberCommand>())).ReturnsAsync(true);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var sut = new ItineraryController(mockMediator.Object, MockSuccessValidation().Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString())
            });

            await sut.AddTeamMember(1, 1);

            mockMediator.Verify(x => x.SendAsync(It.Is<AddTeamMemberCommand>(y => y.ItineraryId == itineraryId && y.TaskSignupId == selectedTeamMember)), Times.Once);
        }

        [Fact]
        public async Task AddTeamMemberDoesNotCallAddTeamMemberCommand_WhenIdIsZero()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AddTeamMemberCommand>())).ReturnsAsync(true);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var sut = new ItineraryController(mockMediator.Object, MockSuccessValidation().Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString())
            });

            await sut.AddTeamMember(0, 1);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<AddTeamMemberCommand>()), Times.Never);
        }

        [Fact]
        public async Task AddTeamMemberDoesNotCallAddTeamMemberCommand_WhenSelectedTeamMemberIsZero()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AddTeamMemberCommand>())).ReturnsAsync(true);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var sut = new ItineraryController(mockMediator.Object, MockSuccessValidation().Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString())
            });

            await sut.AddTeamMember(1, 0);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<AddTeamMemberCommand>()), Times.Never);
        }

        [Fact]
        public async Task AddTeamMemberSendsOrganizationIdQueryWithCorrectItineraryId()
        {
            const int itineraryId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AddTeamMemberCommand>())).ReturnsAsync(true);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var sut = new ItineraryController(mockMediator.Object, MockSuccessValidation().Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString())
            });

            await sut.AddTeamMember(itineraryId, 0);

            mockMediator.Verify(x => x.SendAsync(It.Is<OrganizationIdQuery>(y => y.ItineraryId == itineraryId)), Times.Once);
        }

        [Fact]
        public async Task AddTeamMemberRedirectsToCorrectActionWithCorrectRouteValuesWhenIdIsZeroOrSelectedTeamMemberIsZero()
        {
            const int id = 0;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AddTeamMemberCommand>())).ReturnsAsync(true);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var sut = new ItineraryController(mockMediator.Object, MockSuccessValidation().Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });
            var result = await sut.AddTeamMember(id, 0) as RedirectToActionResult;

            Assert.Equal(result.ActionName, nameof(ItineraryController.Details));
            Assert.Equal(result.RouteValues, new Dictionary<string, object> { ["id"] = id });
        }

        [Fact]
        public async Task AddTeamMemberRedirectsToCorrectActionWithCorrectRouteValuesWhenOrganizationIdIsNotZero_AndUserIsOrgAdmin_AndIdOrSelectedTeamMemberIsNotZero()
        {
            const int id = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AddTeamMemberCommand>())).ReturnsAsync(true);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var sut = new ItineraryController(mockMediator.Object, MockSuccessValidation().Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });
            var result = await sut.AddTeamMember(id, 1) as RedirectToActionResult;

            Assert.Equal(result.ActionName, nameof(ItineraryController.Details));
            Assert.Equal(result.RouteValues, new Dictionary<string, object> { ["id"] = id });
        }

        // todo: sgordon: There should be some tests to validate that org admins for a different org than returned by the OrganizationIdQuery are
        // are unauthorized

        [Fact]
        public void SelectRequestsHasHttpGetAttribute()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            var attribute = sut.GetAttributesOn(x => x.SelectRequests(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void SelectRequestsHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            var routeAttribute = sut.GetAttributesOn(x => x.SelectRequests(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/{id}/[Action]");
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AddRequestsSendsOrganizationIdQueryWithCorrectIntineraryId()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AddRequestsReturnsHttpUnauthorizedWhenOrganizationIdIsZero()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AddRequestsReturnsHttpUnauthorizedWhenUserIsNotOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AddRequestsSendsAddRequestsCommandWhenThereAreSelecteRequests()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AddRequestsRedirectsToCorrectActionWithCorrectRouteValuesWhenOrganizationIdIsNotZero_AndUserIsOrgAdmin_AndThereAreSelectedRequests()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact]
        public void AddRequestsHasHttpPostAttribute()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            var attribute = sut.GetAttributesOn(x => x.AddRequests(It.IsAny<int>(), It.IsAny<string[]>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);        
        }

        [Fact(Skip = "NotImplemented")]
        public void AddRequestsHasRouteAttributeWithCorrectRoute()
        {
        }

        [Fact]
        public void AddRequestsHasValidateAntiForgeryAttribute()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object);
            var routeAttribute = sut.GetAttributesOn(x => x.AddRequests(It.IsAny<int>(), It.IsAny<string[]>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmRemoveTeamMemberSendsOrganizationIdQueryWithTheCorrectItineraryId()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmRemoveTeamMemberReturnsHttpUnauthorizedWhenOrganizationIdIsZero()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmRemoveTeamMemberReturnsHttpUnauthorizedWhenUserIsNotOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmRemoveTeamMemberSendsTaskSignupSummaryQueryWithCorrectTaskSignupIdWhenOrganizationIdIsNotZero_AndUserIsOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmRemoveTeamMemberReturnsHttpNotFoundWhenTaskSignupSummaryModelIsNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmRemoveTeamMemberReturnsTheCorrectViewAndViewModel()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void ConfirmRemoveTeamMemberHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ConfirmRemoveTeamMemberHasRouteAttributeWithCorrectRoute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RemoveTeamMemberSendsOrganizationIdQueryWithCorrectIntineraryId()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RemoveTeamMemberReturnsHttpUnauthorizedWhenOrganizationIdIsZero()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RemoveTeamMemberReturnsHttpUnauthorizedWhenUserIsNotOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RemoveTeamMemberSendsRemoveTeamMemberCommandWithCorrectTaskSignupIdWhenOrganizationIsNotZero_AndUserIsOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RemoveTeamMemberRedirectsToCorrectActionWithCorrectRouteValuesWhenOrganizationIdIsNotZero_AndUserIsOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void RemoveTeamMemberHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void RemoveTeamMemberHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void RemoveTeamMemberHasRouteAttributeWithCorrectRoute()
        {
        }

        #region Helper Methods

        private static void MockMediatorItineraryDetailQuery(out ItineraryController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(null).Verifiable();

            controller = new ItineraryController(mockMediator.Object, MockSuccessValidation().Object);

            return;
        }

        private static Mock<IItineraryEditModelValidator> MockSuccessValidation()
        {
            var mockValidator = new Mock<IItineraryEditModelValidator>();

            mockValidator.Setup(mock => mock.Validate(It.IsAny<ItineraryEditModel>(), It.IsAny<EventSummaryModel>())).Returns(new List<KeyValuePair<string, string>>()).Verifiable();

           return mockValidator;
        }

        private static ItineraryController GetItineraryControllerWithDetailsQuery(string userType, int organizationId)
        {
            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(new ItineraryDetailsModel { Id = 1, Name = "Itinerary", EventId = 1, EventName = "Event Name", OrganizationId = 1, Date = new DateTime(2016, 07, 01) });

            var sut = new ItineraryController(mediator.Object, MockSuccessValidation().Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, userType),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            return sut;
        }

        private static ItineraryController GetItineraryControllerWithEventSummaryQuery(string userType, int organizationId)
        {
            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryModel { Id = 1, Name = "Event", OrganizationId = 1, StartDateTime = new DateTimeOffset(new DateTime(2016, 01, 01)), EndDateTime = new DateTimeOffset(new DateTime(2016, 12, 31)) });
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryCommand>())).ReturnsAsync(1);

            var sut = new ItineraryController(mediator.Object, MockSuccessValidation().Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, userType),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString())
            });

            return sut;
        }

        #endregion
    }
}