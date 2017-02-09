using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.Features.TaskSignups;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Moq;
using Shouldly;
using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AllReady.Caching;
using AllReady.Services.Mapping.Routing;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class ItineraryAdminControllerTests
    {
        [Fact]
        public void Controller_HasAreaAtttribute_WithTheCorrectAreaName()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributes().OfType<AreaAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.RouteValue, "Admin");
        }

        [Fact]
        public void Controller_HasAreaAuthorizeAttribute_WithCorrectPolicy()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Policy, "OrgAdmin");
        }

        [Fact]
        public void DetailsGet_HasHttpGetAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DetailsGet_HasRouteAttribute_WithCorrectRoute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/Details/{id}");
        }

        [Fact]
        public async Task DetailsGet_SendsEventDetailQueryWithCorrectEventId()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(null).Verifiable();

            var sut = new ItineraryController(mockMediator.Object, null, null);
            await sut.Details(1);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>()), Times.Once);
        }

        [Fact]
        public async Task DetailsGet_ReturnsHttpNotFoundResultWhenEventIsNull()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(null).Verifiable();

            var controller = new ItineraryController(mockMediator.Object, null, null);
            Assert.IsType<NotFoundResult>(await controller.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsGet_ReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(new ItineraryDetailsViewModel());

            var sut = new ItineraryController(mediator.Object, null, new FakeUserManager());
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsGet_ReturnsCorrectViewAndViewModel_WhenEventIsNotNullAndUserIsOrgAdmin()
        {
            const int orgId = 1;
            var viewModel = new ItineraryDetailsViewModel { OrganizationId = orgId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(viewModel);

            var sut = new ItineraryController(mediator.Object, null, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.Details(It.IsAny<int>()) as ViewResult;
            Assert.Equal(result.ViewName, "Details");

            var resultViewModel = result.ViewData.Model;
            Assert.IsType<ItineraryDetailsViewModel>(resultViewModel);

            Assert.Equal(resultViewModel, viewModel);
        }

        [Fact]
        public async Task DetailsGet_OptimizeRouteResultStatusQuery_WithExpectedValues()
        {
            OptimizeRouteResultStatusQuery query = null;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(new ItineraryDetailsViewModel { OrganizationId = 1 });
            mediator.Setup(x => x.Send(It.IsAny<OptimizeRouteResultStatusQuery>()))
                .Returns(new OptimizeRouteResultStatus { IsSuccess = true })
                .Callback<OptimizeRouteResultStatusQuery>(x => query = x)                
                .Verifiable();

            var sut = new ItineraryController(mediator.Object, null, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(1.ToString());

            await sut.Details(1);

            mediator.Verify(x => x.Send(It.IsAny<OptimizeRouteResultStatusQuery>()), Times.Once);
            query.ItineraryId.ShouldBe(1);
            query.UserId.ShouldBe("123");
        }

        [Fact]
        public async Task DetailsGet_AddsOptimizeRouteStatusToViewModel_WhenAStatusIsFoundInTheCache()
        {

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(new ItineraryDetailsViewModel { OrganizationId = 1 });
            mediator.Setup(x => x.Send(It.IsAny<OptimizeRouteResultStatusQuery>()))
                .Returns(new OptimizeRouteResultStatus { IsSuccess = true, StatusMessage = "test msg" });

            var sut = new ItineraryController(mediator.Object, null, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(1.ToString());

            var result = await sut.Details(It.IsAny<int>()) as ViewResult;

            var resultViewModel = result.ViewData.Model as ItineraryDetailsViewModel;
            resultViewModel.OptimizeRouteStatus.ShouldNotBeNull();
            resultViewModel.OptimizeRouteStatus.IsSuccess.ShouldBeTrue();
            resultViewModel.OptimizeRouteStatus.StatusMessage.ShouldBe("test msg");
        }

        [Fact]
        public void DetailsPost_HasHttpPostAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<RequestStatus?>()))
                    .OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DetailsPost_HasRouteAttributeWithCorrectRoute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<RequestStatus?>()))
                .OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/Details/{id}");
        }

        [Fact]
        public async Task DetailsPost_SendsEventDetailQueryWithCorrectEventId()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(null).Verifiable();

            var sut = new ItineraryController(mockMediator.Object, null, null);
            await sut.Details(1, null, null);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>()), Times.Once);
        }

        [Fact]
        public async Task DetailsPost_ReturnsHttpNotFoundResultWhenEventIsNull()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(null).Verifiable();

            var controller = new ItineraryController(mockMediator.Object, null, null);
            Assert.IsType<NotFoundResult>(await controller.Details(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<RequestStatus?>()));
        }

        [Fact]
        public async Task DetailsPost_ReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(new ItineraryDetailsViewModel());

            var sut = new ItineraryController(mediator.Object, null, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.Details(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<RequestStatus?>()));
        }

        [Fact]
        public async Task DetailsPost_ReturnsCorrectViewAndViewModelWhenEventIsNotNullAndUserIsOrgAdmin()
        {
            const int orgId = 1;
            var viewModel = new ItineraryDetailsViewModel { OrganizationId = orgId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(viewModel);

            var sut = new ItineraryController(mediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.Details(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<RequestStatus?>()) as ViewResult;
            Assert.Equal(result.ViewName, "Details");

            var resultViewModel = result.ViewData.Model;
            Assert.IsType<ItineraryDetailsViewModel>(resultViewModel);

            Assert.Equal(resultViewModel, viewModel);
        }

        [Fact]
        public async Task DetailsPost_SendsRequestListQueryWithCorrectData()
        {
            const int orgId = 1;
            const int itineraryId = 2;
            const string keywords = "search";
            const RequestStatus status = RequestStatus.Assigned;
            var viewModel = new ItineraryDetailsViewModel { OrganizationId = orgId, Id = 2 };
            var requestList = new List<RequestListViewModel>();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(viewModel);
            mediator.Setup(x => x.SendAsync(It.IsAny<RequestListItemsQuery>())).ReturnsAsync(requestList);

            var sut = new ItineraryController(mediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());
            await sut.Details(itineraryId, keywords, status);

            mediator.Verify(x => x.SendAsync(It.Is<RequestListItemsQuery>(
                y => y.Criteria.ItineraryId == itineraryId &&
                     y.Criteria.Keywords == keywords &&
                     y.Criteria.Status == status)), Times.Once);
        }

        [Fact]
        public async Task DetailsPost_ReturnsViewModelWithCorrectRequestListData()
        {
            const int orgId = 1;
            var fullList = new List<RequestListViewModel> { new RequestListViewModel { Id = Guid.NewGuid() } };
            var filteredList = new List<RequestListViewModel> { new RequestListViewModel { Id = Guid.NewGuid() } };
            var viewModel = new ItineraryDetailsViewModel { OrganizationId = orgId, Requests = fullList };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(viewModel);
            mediator.Setup(x => x.SendAsync(It.IsAny<RequestListItemsQuery>())).ReturnsAsync(filteredList);

            var sut = new ItineraryController(mediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());
            var result = await sut.Details(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<RequestStatus?>()) as ViewResult;

            var model = (ItineraryDetailsViewModel)result.ViewData.Model;

            Assert.Equal(model.Requests, filteredList);
        }

        [Fact]
        public void CreateHasHttpPostAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Create(It.IsAny<ItineraryEditViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void CreateHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.Create(It.IsAny<ItineraryEditViewModel>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/Create");
        }

        [Fact]
        public void CreateHasValidateAntiForgeryAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.Create(It.IsAny<ItineraryEditViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact]
        public async Task CreateReturnsHttpBadRequestWhenModelIsNull()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object, null);
            Assert.IsType<BadRequestResult>(await sut.Create(null));
        }

        [Fact]
        public async Task CreateSendsEventSummaryQueryWithCorrectEventId()
        {
            var model = new ItineraryEditViewModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>()))
                .ReturnsAsync(It.IsAny<EventSummaryViewModel>()).Verifiable();

            var sut = new ItineraryController(mockMediator.Object, MockSuccessValidation().Object, null);
            await sut.Create(model);

            mockMediator.Verify(x => x.SendAsync(It.Is<EventSummaryQuery>(y => y.EventId == model.EventId)), Times.Once);
        }

        [Fact]
        public async Task CreateReturnsHttpBadRequestIfEventNull()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(null);

            var sut = new ItineraryController(mockMediator.Object, MockSuccessValidation().Object, null);
            Assert.IsType<BadRequestResult>(await sut.Create(It.IsAny<ItineraryEditViewModel>()));
        }

        [Fact]
        public async Task CreateReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdminForEventOrg()
        {
            const int orgId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryViewModel { OrganizationId = orgId });

            var sut = new ItineraryController(mediator.Object, MockSuccessValidation().Object, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.Create(new ItineraryEditViewModel()));
        }

        [Fact]
        public async Task CreateReturnsOkResultWhenUserIsOrgAdmin()
        {
            const int orgId = 1;

            //var sut = GetItineraryControllerWithEventSummaryQuery(UserType.OrgAdmin.ToString(), 1);
            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryViewModel { OrganizationId = orgId });
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryCommand>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mediator.Object, MockSuccessValidation().Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            Assert.IsType<OkObjectResult>(await sut.Create(new ItineraryEditViewModel()));
        }

        [Fact]
        public async Task CreateReturnsOkResultWhenUserIsSiteAdmin_AndModelIsValid_AndSuccessfulAdd()
        {
            const int orgId = 1;

            //var sut = GetItineraryControllerWithEventSummaryQuery(UserType.SiteAdmin.ToString(), 0);
            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryViewModel { OrganizationId = orgId });
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryCommand>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mediator.Object, MockSuccessValidation().Object, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "0")
            });

            Assert.IsType<OkObjectResult>(await sut.Create(new ItineraryEditViewModel()));
        }

        [Fact]
        public async Task CreateReturnsHttpBadRequestResultWhenModelStateHasError()
        {
            const int orgId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryViewModel { OrganizationId = orgId });

            var sut = new ItineraryController(mediator.Object, MockSuccessValidation().Object, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());
            sut.AddModelStateError();

            var result = await sut.Create(new ItineraryEditViewModel());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateCallsValidatorWithCorrectItineraryEditModelAndEventSummaryModel()
        {
            var model = new ItineraryEditViewModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var mediator = new Mock<IMediator>();

            var eventSummaryModel = new EventSummaryViewModel
            {
                Id = 1,
                Name = "Event",
                OrganizationId = 1,
                StartDateTime = new DateTimeOffset(new DateTime(2016, 01, 01)),
                EndDateTime = new DateTimeOffset(new DateTime(2016, 12, 31))
            };
            mediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(eventSummaryModel);
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryCommand>())).ReturnsAsync(1);

            var mockValidator = new Mock<IItineraryEditModelValidator>();
            mockValidator.Setup(mock => mock.Validate(model, eventSummaryModel)).Returns(new List<KeyValuePair<string, string>>()).Verifiable();

            var sut = new ItineraryController(mediator.Object, mockValidator.Object, null);
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
            var model = new ItineraryEditViewModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryViewModel { Id = 1, Name = "Event", OrganizationId = 1, StartDateTime = new DateTimeOffset(new DateTime(2016, 01, 01)), EndDateTime = new DateTimeOffset(new DateTime(2016, 12, 31)) });
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryCommand>())).ReturnsAsync(1);

            var validatorError = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key", "value")
            };

            var mockValidator = new Mock<IItineraryEditModelValidator>();
            mockValidator.Setup(mock => mock.Validate(It.IsAny<ItineraryEditViewModel>(), It.IsAny<EventSummaryViewModel>())).Returns(validatorError).Verifiable();

            var sut = new ItineraryController(mediator.Object, mockValidator.Object, null);
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
            var model = new ItineraryEditViewModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryViewModel { Id = 1, Name = "Event", OrganizationId = 1, StartDateTime = new DateTimeOffset(new DateTime(2016, 01, 01)), EndDateTime = new DateTimeOffset(new DateTime(2016, 12, 31)) });
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryCommand>())).ReturnsAsync(0).Verifiable();

            var mockValidator = new Mock<IItineraryEditModelValidator>();
            mockValidator.Setup(mock => mock.Validate(It.IsAny<ItineraryEditViewModel>(), It.IsAny<EventSummaryViewModel>())).Returns(new List<KeyValuePair<string, string>>()).Verifiable();

            var sut = new ItineraryController(mediator.Object, mockValidator.Object, null);
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
            var model = new ItineraryEditViewModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryViewModel { Id = 1, Name = "Event", OrganizationId = 1, StartDateTime = new DateTimeOffset(new DateTime(2016, 01, 01)), EndDateTime = new DateTimeOffset(new DateTime(2016, 12, 31)) });
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryCommand>())).ReturnsAsync(0);

            var mockValidator = new Mock<IItineraryEditModelValidator>();
            mockValidator.Setup(mock => mock.Validate(It.IsAny<ItineraryEditViewModel>(), It.IsAny<EventSummaryViewModel>())).Returns(new List<KeyValuePair<string, string>>()).Verifiable();

            var sut = new ItineraryController(mediator.Object, mockValidator.Object, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            var result = await sut.Create(model);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void EditGet_HasRouteAttribute_WithCorrectRoute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/Edit/{id}");
        }

        [Fact]
        public async Task EditGet_SendsEditItineraryQuery_WithCorrectId()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryQuery>())).ReturnsAsync(null).Verifiable();

            var sut = new ItineraryController(mediator.Object, Mock.Of<IItineraryEditModelValidator>(), null);
            await sut.Edit(1);

            mediator.Verify(x => x.SendAsync(It.Is<EditItineraryQuery>(y => y.ItineraryId == 1)), Times.Once);
        }

        [Fact]
        public async Task EditGet_ReturnsBadResult_WhenEditItineraryQuery_ReturnsNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryQuery>())).ReturnsAsync(null);

            var sut = new ItineraryController(mediator.Object, Mock.Of<IItineraryEditModelValidator>(), null);

            Assert.IsType<BadRequestResult>(await sut.Edit(1));
        }

        [Fact]
        public async Task EditGet_ReturnsHttpUnauthorizedResult_WhenUserIsNotOrgAdminForEventOrg()
        {
            const int orgId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryQuery>())).ReturnsAsync(new ItineraryEditViewModel { OrganizationId = orgId });

            var sut = new ItineraryController(mediator.Object, MockSuccessValidation().Object, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.Edit(1));
        }

        [Fact]
        public async Task EditGet_ReturnsViewResult_WhenUserIsSiteAdmin()
        {
            const int orgId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryQuery>())).ReturnsAsync(new ItineraryEditViewModel { OrganizationId = orgId, Id = 100, Name = "Test" });

            var sut = new ItineraryController(mediator.Object, MockSuccessValidation().Object, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "0")
            });

            // Assert
            var viewResult = Assert.IsType<ViewResult>(await sut.Edit(1));
            var model = viewResult.Model as ItineraryEditViewModel;

            model.ShouldNotBeNull();
            model.Name.ShouldBe("Test");
        }

        [Fact]
        public void EditPost_HasValidateAntiForgeryAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<ItineraryEditViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact]
        public async Task EditPost_ReturnsHttpBadRequestWhenModelIsNull()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), MockSuccessValidation().Object, null);
            Assert.IsType<BadRequestResult>(await sut.Edit(null));
        }

        [Fact]
        public void EditPost_HasRouteAttribute_WithCorrectRoute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<ItineraryEditViewModel>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/Edit/{id}");
        }

        [Fact]
        public async Task EditPost_SendsItinerarySummaryQuery_WithCorrectId()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ItinerarySummaryQuery>())).ReturnsAsync(null).Verifiable();

            var sut = new ItineraryController(mediator.Object, Mock.Of<IItineraryEditModelValidator>(), null);
            await sut.Edit(new ItineraryEditViewModel { Id = 50 });

            mediator.Verify(x => x.SendAsync(It.Is<ItinerarySummaryQuery>(y => y.ItineraryId == 50)), Times.Once);
        }

        [Fact]
        public async Task EditPost_ReturnsBadResult_WhenEditItineraryQuery_ReturnsNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ItinerarySummaryQuery>())).ReturnsAsync(null);

            var sut = new ItineraryController(mediator.Object, Mock.Of<IItineraryEditModelValidator>(), null);

            Assert.IsType<BadRequestResult>(await sut.Edit(new ItineraryEditViewModel { Id = 50 }));
        }

        [Fact]
        public async Task EditPost_ReturnsHttpUnauthorizedResult_WhenUserIsNotOrgAdminForEventOrg()
        {
            const int orgId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ItinerarySummaryQuery>())).ReturnsAsync(new ItinerarySummaryViewModel { OrganizationId = orgId });

            var sut = new ItineraryController(mediator.Object, MockSuccessValidation().Object, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.Edit(new ItineraryEditViewModel { Id = 50 }));
        }

        [Fact]
        public async Task EditPost_CallsValidator_WithCorrectProperties()
        {
            var model = new ItineraryEditViewModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var mediator = new Mock<IMediator>();

            var itinerarySummaryModel = new ItinerarySummaryViewModel
            {
                Id = 1,
                Name = "Itinerary",
                OrganizationId = 1,
                EventSummary = new EventSummaryViewModel
                {
                    StartDateTime = new DateTimeOffset(new DateTime(2016, 01, 01)),
                    EndDateTime = new DateTimeOffset(new DateTime(2016, 12, 31))
                }
            };
            mediator.Setup(x => x.SendAsync(It.IsAny<ItinerarySummaryQuery>())).ReturnsAsync(itinerarySummaryModel);

            var mockValidator = new Mock<IItineraryEditModelValidator>();
            mockValidator.Setup(mock => mock.Validate(model, itinerarySummaryModel.EventSummary)).Returns(new List<KeyValuePair<string, string>>()).Verifiable();

            var sut = new ItineraryController(mediator.Object, mockValidator.Object, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            await sut.Edit(model);

            mockValidator.Verify(x => x.Validate(model, itinerarySummaryModel.EventSummary), Times.Once);
        }

        [Fact]
        public async Task EditPost_ReturnsViewWithModel_WhenValidatonFails()
        {
            var model = new ItineraryEditViewModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var mediator = new Mock<IMediator>();

            var itinerarySummaryModel = new ItinerarySummaryViewModel
            {
                Id = 1,
                Name = "Itinerary",
                OrganizationId = 1,
                EventSummary = new EventSummaryViewModel
                {
                    StartDateTime = new DateTimeOffset(new DateTime(2016, 01, 01)),
                    EndDateTime = new DateTimeOffset(new DateTime(2016, 12, 31))
                }
            };
            mediator.Setup(x => x.SendAsync(It.IsAny<ItinerarySummaryQuery>())).ReturnsAsync(itinerarySummaryModel);

            var validatorError = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key", "value")
            };

            var mockValidator = new Mock<IItineraryEditModelValidator>();
            mockValidator.Setup(mock => mock.Validate(It.IsAny<ItineraryEditViewModel>(), It.IsAny<EventSummaryViewModel>())).Returns(validatorError);

            var sut = new ItineraryController(mediator.Object, mockValidator.Object, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            var result = await sut.Edit(model);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultViewModel = viewResult.ViewData.Model;
            Assert.IsType<ItineraryEditViewModel>(resultViewModel);

            Assert.Equal(resultViewModel, model);
        }

        [Fact]
        public async Task EditPost_SendsEditItineraryCommand_WithCorrectItineraryEditModel()
        {
            var model = new ItineraryEditViewModel
            {
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var mediator = new Mock<IMediator>();

            var itinerarySummaryModel = new ItinerarySummaryViewModel
            {
                Id = 1,
                Name = "Itinerary",
                OrganizationId = 1,
                EventSummary = new EventSummaryViewModel
                {
                    StartDateTime = new DateTimeOffset(new DateTime(2016, 01, 01)),
                    EndDateTime = new DateTimeOffset(new DateTime(2016, 12, 31))
                }
            };
            mediator.Setup(x => x.SendAsync(It.IsAny<ItinerarySummaryQuery>())).ReturnsAsync(itinerarySummaryModel);
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryCommand>())).ReturnsAsync(0).Verifiable();

            var mockValidator = new Mock<IItineraryEditModelValidator>();
            mockValidator.Setup(mock => mock.Validate(It.IsAny<ItineraryEditViewModel>(), It.IsAny<EventSummaryViewModel>())).Returns(new List<KeyValuePair<string, string>>());

            var sut = new ItineraryController(mediator.Object, mockValidator.Object, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            await sut.Edit(model);

            mediator.Verify(x => x.SendAsync(It.Is<EditItineraryCommand>(y => y.Itinerary == model)), Times.Once);
        }

        [Fact]
        public async Task EditPost_ReturnsRedirect_AfterCommandIsSent()
        {
            var model = new ItineraryEditViewModel
            {
                Id = 1,
                EventId = 1,
                Name = "Test",
                Date = new DateTime(2016, 06, 01)
            };

            var mediator = new Mock<IMediator>();

            var itinerarySummaryModel = new ItinerarySummaryViewModel
            {
                Id = 1,
                Name = "Itinerary",
                OrganizationId = 1,
                EventSummary = new EventSummaryViewModel
                {
                    StartDateTime = new DateTimeOffset(new DateTime(2016, 01, 01)),
                    EndDateTime = new DateTimeOffset(new DateTime(2016, 12, 31))
                }
            };
            mediator.Setup(x => x.SendAsync(It.IsAny<ItinerarySummaryQuery>())).ReturnsAsync(itinerarySummaryModel);
            mediator.Setup(x => x.SendAsync(It.IsAny<EditItineraryCommand>())).ReturnsAsync(0);

            var mockValidator = new Mock<IItineraryEditModelValidator>();
            mockValidator.Setup(mock => mock.Validate(It.IsAny<ItineraryEditViewModel>(), It.IsAny<EventSummaryViewModel>())).Returns(new List<KeyValuePair<string, string>>());

            var sut = new ItineraryController(mediator.Object, mockValidator.Object, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            var result = await sut.Edit(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            redirectResult.ActionName.ShouldBe("Details");

            var id = redirectResult.RouteValues["id"];
            id.ShouldNotBeNull();
            id.ShouldBe(model.Id);
        }

        [Fact]
        public void AddTeamMemberHasHttpPostAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.AddTeamMember(It.IsAny<int>(), It.IsAny<int>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void AddTeamMemberHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.AddTeamMember(It.IsAny<int>(), It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/AddTeamMember");
        }

        [Fact]
        public void AddTeamMemberHasValidateAntiForgeryAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.AddTeamMember(It.IsAny<int>(), It.IsAny<int>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact]
        public async Task AddTeamMemberSendsAddTeamMemberCommandWithCorrectParameters()
        {
            const int itineraryId = 1;
            const int selectedTeamMember = 1;
            string volunteerEmail = "test@example.com";

            var mockMediator = new Mock<IMediator>();
            var itineraryDetailsViewModel = new ItineraryDetailsViewModel
            {
                PotentialTeamMembers = new List<SelectListItem>(),
                TeamMembers = new List<TeamListViewModel>
                {
                    new TeamListViewModel {VolunteerEmail = volunteerEmail}
                }
            };
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AddTeamMemberCommand>())).ReturnsAsync(true);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(itineraryDetailsViewModel);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString())
            });

            await sut.AddTeamMember(1, 1);

            mockMediator.Verify(x => x.SendAsync(It.Is<AddTeamMemberCommand>(y => y.ItineraryId == itineraryId && y.VolunteerTaskSignupId == selectedTeamMember)), Times.Once);
            mockMediator.Verify(x => x.SendAsync(It.Is<ItineraryDetailQuery>(y => y.ItineraryId == itineraryId)), Times.Once);
        }

        [Fact]
        public async Task AddTeamMemberDoesNotCallAddTeamMemberCommand_WhenIdIsZero()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AddTeamMemberCommand>())).ReturnsAsync(true);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var sut = new ItineraryController(mockMediator.Object, null, null);
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

            var sut = new ItineraryController(mockMediator.Object, null, null);
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

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString())
            });

            await sut.AddTeamMember(itineraryId, 0);

            mockMediator.Verify(x => x.SendAsync(It.Is<OrganizationIdQuery>(y => y.ItineraryId == itineraryId)), Times.Once);
        }

        [Fact]
        public async Task AddTeamMemberReturnsCorrectJsonWhenIdIsZeroOrSelectedTeamMemberIsZero()
        {
            const int itineraryId = 0;
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AddTeamMemberCommand>())).ReturnsAsync(true);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());
            var jsonResult = await sut.AddTeamMember(itineraryId, 0) as JsonResult;

            var successStatus = jsonResult.GetValueForProperty<bool>("isSuccess");
            var errors = jsonResult.GetValueForProperty<string[]>("errors");

            Assert.False(successStatus);
            Assert.NotNull(errors);
            Assert.Equal(1, errors.Length);
            Assert.Equal("Invalid selection.", errors[0]);
        }

        [Fact]
        public async Task AddTeamMemberReturnsCorrectJsonWhenOrganizationIdIsNotZero_AndUserIsOrgAdmin_AndIdOrSelectedTeamMemberIsNotZero()
        {
            const int itineraryId = 1;
            const int orgId = 1;
            const int teamMemberId = 1;
            string volunteerEmail = "test@example.com";

            var mockMediator = new Mock<IMediator>();
            var itineraryDetailsViewModel = new ItineraryDetailsViewModel
            {
                PotentialTeamMembers = new List<SelectListItem>(),
                TeamMembers = new List<TeamListViewModel>
                {
                    new TeamListViewModel {VolunteerEmail = volunteerEmail}
                }
            };
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AddTeamMemberCommand>())).ReturnsAsync(true);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<ItineraryDetailQuery>())).ReturnsAsync(itineraryDetailsViewModel);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());
            var jsonResult = await sut.AddTeamMember(itineraryId, teamMemberId) as JsonResult;

            var successStatus = jsonResult.GetValueForProperty<bool>("isSuccess");
            var teamMembers = jsonResult.GetValueForProperty<IEnumerable<TeamListViewModel>>("teamMembers").ToList();
            var potentialMembers = jsonResult.GetValueForProperty<IEnumerable<SelectListItem>>("potentialTeamMembers").ToList();

            Assert.True(successStatus);
            Assert.NotNull(teamMembers);
            Assert.NotNull(potentialMembers);
            Assert.Equal(1, teamMembers.Count);
            Assert.Equal(volunteerEmail, teamMembers[0].VolunteerEmail);
        }

        // todo: sgordon: There should be some tests to validate that org admins for a different org than returned by the OrganizationIdQuery are
        // are unauthorized

        [Fact]
        public void SelectRequestsGetHasHttpGetAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.SelectRequests(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void SelectRequestsGetHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.SelectRequests(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/{id}/[Action]");
        }

        [Fact]
        public async Task SelectRequestsGetSendsOrganizationIdQueryWithCorrectItineraryId()
        {
            const int itineraryId = 1;

            var mediator = new Mock<IMediator>();

            var sut = new ItineraryController(mediator.Object, null, null);
            await sut.SelectRequests(itineraryId);

            mediator.Verify(x => x.SendAsync(It.Is<OrganizationIdQuery>(y => y.ItineraryId == itineraryId)));
        }

        [Fact]
        public async Task SelectRequestsGetReturnsUnauthorizedResult_WhenOrgIdIsZero()
        {
            var sut = new ItineraryController(Mock.Of<IMediator>(), null, null);

            var result = await sut.SelectRequests(It.IsAny<int>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SelectRequestsGetReturnsUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var sut = new ItineraryController(mediator.Object, null, null);
            sut.MakeUserNotAnOrgAdmin();

            var result = await sut.SelectRequests(It.IsAny<int>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SelectRequestsPostWithSingleParameterSetsSelectItineraryRequestsModelWithTheCorrectData()
        {
            const int organizationId = 4;

            var itinerary = GetItineraryForSelectRequestHappyPathTests();
            var returnedRequests = GetRequestsForSelectRequestHappyPathTests();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<ItineraryDetailQuery>(y => y.ItineraryId == itinerary.Id))).ReturnsAsync(itinerary);
            mediator.Setup(x => x.SendAsync(It.IsAny<RequestListItemsQuery>())).ReturnsAsync(returnedRequests);
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(organizationId);

            var sut = new ItineraryController(mediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var view = await sut.SelectRequests(itinerary.Id, new SelectItineraryRequestsViewModel());

            RunSelectRequestsHappyPathTests(view, itinerary, returnedRequests);
        }

        [Fact]
        public async Task SelectRequestsPostWithTwoParametersSetsSelectItineraryRequestsModelWithTheCorrectData()
        {
            var itineraryRequestsModel = new SelectItineraryRequestsViewModel { KeywordsFilter = "These are keywords" };
            var itinerary = GetItineraryForSelectRequestHappyPathTests();
            var returnedRequests = GetRequestsForSelectRequestHappyPathTests();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<ItineraryDetailQuery>(y => y.ItineraryId == itinerary.Id)))
                    .ReturnsAsync(itinerary);
            mediator.Setup(x => x.SendAsync(It.Is<RequestListItemsQuery>(y =>
                y.Criteria.Keywords.Equals(itineraryRequestsModel.KeywordsFilter) &&
                y.Criteria.EventId == itinerary.EventId)))
                .ReturnsAsync(returnedRequests);

            var sut = new ItineraryController(mediator.Object, null, null);

            var view = await sut.SelectRequests(itinerary.Id, itineraryRequestsModel);

            RunSelectRequestsHappyPathTests(view, itinerary, returnedRequests);
        }

        [Fact]
        public async Task AddRequestsSendsOrganizationIdQueryWithCorrectItineraryId()
        {
            const int itineraryId = 1;
            var mockMediator = new Mock<IMediator>();

            var sut = new ItineraryController(mockMediator.Object, null, null);

            await sut.AddRequests(itineraryId, It.IsAny<List<string>>());

            mockMediator.Verify(x => x.SendAsync(It.Is<OrganizationIdQuery>(y => y.ItineraryId == itineraryId)), Times.Once);
        }

        [Fact]
        public async Task AddRequestsReturnsHttpUnauthorizedWhenOrganizationIdIsZero()
        {
            var itineraryId = It.IsAny<int>();
            var selectedRequests = new List<string> { "request1", "request2" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(0);

            var sut = new ItineraryController(mockMediator.Object, null, null);

            Assert.IsType<UnauthorizedResult>(await sut.AddRequests(itineraryId, selectedRequests));
        }

        [Fact]
        public async Task AddRequestsReturnsHttpUnauthorizedWhenUserIsNotOrgAdmin()
        {
            var itineraryId = It.IsAny<int>();
            var selectedRequests = new List<string> { "request1", "request2" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.AddRequests(itineraryId, selectedRequests));
        }

        [Fact]
        public async Task AddRequestsSendsAddRequestsCommandWhenThereAreSelectedRequests()
        {
            const int orgId = 1;

            var itineraryId = It.IsAny<int>();
            var selectedRequests = new List<string> { "request1", "request2" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AddRequestsToItineraryCommand>())).ReturnsAsync(true);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            await sut.AddRequests(itineraryId, selectedRequests);

            mockMediator.Verify(x => x.SendAsync(It.Is<AddRequestsToItineraryCommand>(y => y.RequestIdsToAdd.Equals(selectedRequests))), Times.Once);
        }

        [Fact]
        public async Task AddRequestsRedirectsToCorrectActionWithCorrectRouteValuesWhenOrganizationIdIsNotZero_AndUserIsOrgAdmin_AndThereAreSelectedRequests()
        {
            const int orgId = 1;

            var itineraryId = It.IsAny<int>();
            var selectedRequests = new List<string> { "request1", "request2" };

            var mockMediator = new Mock<IMediator>();

            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.AddRequests(itineraryId, selectedRequests) as RedirectToActionResult;

            Assert.Equal(result.ActionName, nameof(ItineraryController.Details));
            Assert.Equal(result.RouteValues, new Dictionary<string, object> { ["id"] = itineraryId });
        }

        [Fact]
        public void AddRequestsHasHttpPostAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.AddRequests(It.IsAny<int>(), It.IsAny<List<string>>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void AddRequestsHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.AddRequests(It.IsAny<int>(), It.IsAny<List<string>>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/{id}/[Action]");
        }

        [Fact]
        public void AddRequestsHasValidateAntiForgeryAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.AddRequests(It.IsAny<int>(), It.IsAny<List<string>>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
        }

        [Fact]
        public async Task ConfirmRemoveRequestSendsOrganizationIdQueryWithCorrectItineraryId()
        {
            const int itineraryId = 1;

            var mediator = new Mock<IMediator>();

            var sut = new ItineraryController(mediator.Object, null, null);
            await sut.ConfirmRemoveRequest(itineraryId, It.IsAny<Guid>());

            mediator.Verify(x => x.SendAsync(It.Is<OrganizationIdQuery>(y => y.ItineraryId == itineraryId)), Times.Once);
        }

        [Fact]
        public async Task ConfirmRemoveRequestReturnsUnauthorizedResult_WhenOrgIdIsZero()
        {
            const int orgId = 0;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mediator.Object, null, null);
            var result = await sut.ConfirmRemoveRequest(It.IsAny<int>(), It.IsAny<Guid>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task ConfirmRemoveRequestReturnsUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var sut = new ItineraryController(mediator.Object, null, null);
            sut.MakeUserNotAnOrgAdmin();

            var result = await sut.ConfirmRemoveRequest(It.IsAny<int>(), It.IsAny<Guid>());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task ConfirmRemoveRequestReturnsNotFoundResult_WhenViewModelIsNull()
        {
            const int orgId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.ConfirmRemoveRequest(It.IsAny<int>(), It.IsAny<Guid>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ConfirmRemoveRequestSendsRequestSummaryQueryWithCorrectRequestId()
        {
            const int orgId = 1;
            var requestId = Guid.NewGuid();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            await sut.ConfirmRemoveRequest(It.IsAny<int>(), requestId);

            mediator.Verify(x => x.SendAsync(It.Is<RequestSummaryQuery>(y => y.RequestId == requestId)));
        }

        [Fact]
        public async Task ConfirmRemoveRequestAssignsCorrectTitleOnViewModel()
        {
            const int orgId = 1;
            var requestId = Guid.NewGuid();
            var viewModel = new RequestSummaryViewModel { Name = "ViewModelName" };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);
            mediator.Setup(x => x.SendAsync(It.IsAny<RequestSummaryQuery>())).ReturnsAsync(viewModel);

            var sut = new ItineraryController(mediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.ConfirmRemoveRequest(It.IsAny<int>(), requestId) as ViewResult;
            var resultViewModel = result.ViewData.Model as RequestSummaryViewModel;

            Assert.Equal(resultViewModel.Title, $"Remove request: {viewModel.Name}");
        }

        [Fact]
        public async Task ConfirmRemoveRequestSetsUserIsOrgAdminToTrue_WhenUserIsOrgAdmin()
        {
            const int orgId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);
            mediator.Setup(x => x.SendAsync(It.IsAny<RequestSummaryQuery>())).ReturnsAsync(new RequestSummaryViewModel());

            var sut = new ItineraryController(mediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.ConfirmRemoveRequest(It.IsAny<int>(), It.IsAny<Guid>()) as ViewResult;
            var resultViewModel = result.ViewData.Model as RequestSummaryViewModel;

            Assert.True(resultViewModel.UserIsOrgAdmin);
        }

        [Fact]
        public async Task ConfirmRemoveRequestRetrunsCorrectViewAndViewModel()
        {
            const int orgId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);
            mediator.Setup(x => x.SendAsync(It.IsAny<RequestSummaryQuery>())).ReturnsAsync(new RequestSummaryViewModel());

            var sut = new ItineraryController(mediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.ConfirmRemoveRequest(It.IsAny<int>(), It.IsAny<Guid>()) as ViewResult;
            var resultViewModel = result.ViewData.Model as RequestSummaryViewModel;

            Assert.Equal(result.ViewName, "ConfirmRemoveRequest");
            Assert.IsType<RequestSummaryViewModel>(resultViewModel);
        }

        [Fact]
        public void ConfirmRemoveRequestHasHttpGetAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.ConfirmRemoveRequest(It.IsAny<int>(), It.IsAny<Guid>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ConfirmRemoveRequestHasRouteAttributeWithTheCorrectRouteValule()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.ConfirmRemoveRequest(It.IsAny<int>(), It.IsAny<Guid>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "Admin/Itinerary/{itineraryId}/[Action]/{requestId}");
        }

        [Fact]
        public async Task RemoveRequestReturnsUnathorizedResult_WhenUserIsNotOrgAdmin()
        {
            var sut = new ItineraryController(null, null, null);
            var result = await sut.RemoveRequest(new RequestSummaryViewModel { UserIsOrgAdmin = false });
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task RemoveRequestSendsRemoveRequestCommandWithCorrectData()
        {
            var viewModel = new RequestSummaryViewModel { Id = Guid.NewGuid(), ItineraryId = 1, UserIsOrgAdmin = true };

            var mediator = new Mock<IMediator>();
            var sut = new ItineraryController(mediator.Object, null, null);

            await sut.RemoveRequest(viewModel);

            mediator.Verify(x => x.SendAsync(It.Is<RemoveRequestCommand>(y => y.RequestId == viewModel.Id && y.ItineraryId == viewModel.ItineraryId)));
        }

        [Fact]
        public async Task RemoveRequestRedirectsToCorrectActionWithCorrrectRouteValues()
        {
            var viewModel = new RequestSummaryViewModel { ItineraryId = 1, UserIsOrgAdmin = true };

            var mediator = new Mock<IMediator>();
            var sut = new ItineraryController(mediator.Object, null, null);

            var result = await sut.RemoveRequest(viewModel) as RedirectToActionResult;

            var routeValueDictionary = new RouteValueDictionary { ["id"] = viewModel.ItineraryId };

            Assert.Equal(result.ActionName, "Details");
            Assert.Equal(result.RouteValues, routeValueDictionary);
        }

        [Fact]
        public void RemoveRequestHasHttpPostAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.RemoveRequest(It.IsAny<RequestSummaryViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RemoveRequestHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.RemoveRequest(It.IsAny<RequestSummaryViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RemoveRequestHasRouteAttributeWithCorrectRouteValue()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.RemoveRequest(It.IsAny<RequestSummaryViewModel>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "Admin/Itinerary/{itineraryId}/[Action]/{requestId}");
        }

        [Fact]
        public async Task ConfirmRemoveTeamMemberSendsOrganizationIdQueryWithTheCorrectItineraryId()
        {
            const int itineraryId = 1;

            var mockMediator = new Mock<IMediator>();

            var sut = new ItineraryController(mockMediator.Object, null, null);

            await sut.ConfirmRemoveTeamMember(itineraryId, It.IsAny<int>());

            mockMediator.Verify(x => x.SendAsync(It.Is<OrganizationIdQuery>(y => y.ItineraryId == itineraryId)), Times.Once);
        }

        [Fact]
        public async Task ConfirmRemoveTeamMemberReturnsHttpUnauthorized_WhenOrganizationIdIsZero()
        {
            var itineraryId = It.IsAny<int>();
            var volunteerTaskSignupId = It.IsAny<int>();

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(0);

            var sut = new ItineraryController(mockMediator.Object, null, null);

            Assert.IsType<UnauthorizedResult>(await sut.ConfirmRemoveTeamMember(itineraryId, volunteerTaskSignupId));
        }

        [Fact]
        public async Task ConfirmRemoveTeamMemberReturnsHttpUnauthorized_WhenUserIsNotOrgAdmin()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.ConfirmRemoveTeamMember(It.IsAny<int>(), It.IsAny<int>()));
        }

        [Fact]
        public async Task ConfirmRemoveTeamMemberSendsTaskSignupSummaryQueryWithCorrectTaskSignupIdWhenOrganizationIdIsNotZero_AndUserIsOrgAdmin()
        {
            const int orgId = 1;
            const int volunteerTaskSignupId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            await sut.ConfirmRemoveTeamMember(It.IsAny<int>(), volunteerTaskSignupId);

            mockMediator.Verify(x => x.SendAsync(It.Is<TaskSignupSummaryQuery>(y => y.VolunteerTaskSignupId == volunteerTaskSignupId)), Times.Once);
        }

        [Fact]
        public async Task ConfirmRemoveTeamMemberReturnsHttpNotFound_WhenTaskSignupSummaryModelIsNull()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<TaskSignupSummaryQuery>())).ReturnsAsync(null);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            Assert.IsType<NotFoundResult>(await sut.ConfirmRemoveTeamMember(It.IsAny<int>(), It.IsAny<int>()));
        }

        [Fact]
        public async Task ConfirmRemoveTeamMemberReturnsTheCorrectViewAndViewModel()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<TaskSignupSummaryQuery>())).ReturnsAsync(new TaskSignupSummaryViewModel { VolunteerTaskSignupId = It.IsAny<int>(), VolunteerEmail = "user@domain.tld", VolunteerName = "Test McTesterson" });

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.ConfirmRemoveTeamMember(It.IsAny<int>(), It.IsAny<int>()) as ViewResult;

            Assert.Equal(result.ViewName, "ConfirmRemoveTeamMember");

            var resultViewModel = result.ViewData.Model;
            Assert.IsType<TaskSignupSummaryViewModel>(resultViewModel);
        }

        [Fact]
        public void ConfirmRemoveTeamMemberHasHttpGetAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.ConfirmRemoveTeamMember(It.IsAny<int>(), It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ConfirmRemoveTeamMemberHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.ConfirmRemoveTeamMember(It.IsAny<int>(), It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/{itineraryId}/[Action]/{taskSignupId}");
        }

        [Fact]
        public async Task RemoveTeamMemberReturnsHttpUnauthorizedWhenUserIsNotOrgAdmin()
        {
            var viewModel = new TaskSignupSummaryViewModel { UserIsOrgAdmin = false };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var sut = new ItineraryController(mockMediator.Object, null, null);

            Assert.IsType<UnauthorizedResult>(await sut.RemoveTeamMember(viewModel));
        }

        [Fact]
        public async Task RemoveTeamMemberSendsRemoveTeamMemberCommandWithCorrectTaskSignupId_WhenOrganizationIsNotZero_AndUserIsOrgAdmin()
        {
            const int volunteerTaskSignupId = 1;

            var viewModel = new TaskSignupSummaryViewModel { VolunteerTaskSignupId = volunteerTaskSignupId, UserIsOrgAdmin = true };

            var mockMediator = new Mock<IMediator>();

            var sut = new ItineraryController(mockMediator.Object, null, null);

            await sut.RemoveTeamMember(viewModel);

            mockMediator.Verify(x => x.SendAsync(It.Is<RemoveTeamMemberCommand>(y => y.VolunteerTaskSignupId == volunteerTaskSignupId)), Times.Once);
        }

        [Fact]
        public async Task RemoveTeamMemberRedirectsToCorrectActionWithCorrectRouteValues_WhenOrganizationIdIsNotZero_AndUserIsOrgAdmin()
        {
            const int itineraryId = 1;
            var viewModel = new TaskSignupSummaryViewModel { ItineraryId = itineraryId, UserIsOrgAdmin = true };
            var routeValueDictionary = new RouteValueDictionary { ["id"] = viewModel.ItineraryId };

            var mediator = new Mock<IMediator>();

            var sut = new ItineraryController(mediator.Object, null, null);
            var result = await sut.RemoveTeamMember(viewModel) as RedirectToActionResult;

            Assert.Equal(result.ActionName, nameof(ItineraryController.Details));
            Assert.Equal(result.RouteValues, routeValueDictionary);
        }

        [Fact]
        public void RemoveTeamMemberHasHttpPostAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.RemoveTeamMember(It.IsAny<TaskSignupSummaryViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RemoveTeamMemberHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.RemoveTeamMember(It.IsAny<TaskSignupSummaryViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RemoveTeamMemberHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.RemoveTeamMember(It.IsAny<TaskSignupSummaryViewModel>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/{itineraryId}/[Action]/{taskSignupId}");
        }

        [Fact]
        public void MarkCompleteHasHttpPostAttribute()
        {
            var sut = new ItineraryController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.MarkComplete(It.IsAny<int>(), It.IsAny<Guid>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void MarkCompleteHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ItineraryController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.MarkComplete(It.IsAny<int>(), It.IsAny<Guid>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/{itineraryId}/[Action]/{requestId}");
        }

        [Fact]
        public async Task MarkCompleteSendsOrganizationIdQueryWithCorrectItineraryId()
        {
            var itineraryId = It.IsAny<int>();
            var mockMediator = new Mock<IMediator>();

            var sut = new ItineraryController(mockMediator.Object, null, null);

            await sut.MarkComplete(itineraryId, It.IsAny<Guid>());

            mockMediator.Verify(x => x.SendAsync(It.Is<OrganizationIdQuery>(y => y.ItineraryId == itineraryId)), Times.Once);
        }

        [Fact]
        public async Task MarkCompleteReturnsHttpUnauthorizedWhenOrganizationIdIsZero()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(0);

            var sut = new ItineraryController(mockMediator.Object, null, null);

            Assert.IsType<UnauthorizedResult>(await sut.MarkComplete(It.IsAny<int>(), It.IsAny<Guid>()));
        }

        [Fact]
        public async Task MarkCompleteReturnsHttpUnauthorizedWhenUserIsNotOrgAdmin()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await sut.MarkComplete(It.IsAny<int>(), It.IsAny<Guid>()));
        }

        [Fact]
        public async Task MarkCompleteSendsRequestStatusChangeCommandWithCorrectRequestIdWhenOrganizationIsNotZero_AndUserIsOrgAdmin()
        {
            var requestId = Guid.NewGuid();
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            await sut.MarkComplete(It.IsAny<int>(), requestId);

            mockMediator.Verify(x => x.SendAsync(It.Is<ChangeRequestStatusCommand>(y => y.RequestId == requestId)), Times.Once);
        }

        [Fact]
        public async Task MarkCompleteReturnsRedirectToActionWhenOrganizationIsNotZero_AndUserIsOrgAdmin()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.MarkComplete(It.IsAny<int>(), It.IsAny<Guid>());
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task MarkCompleteReturnsRedirectToAction_WithActionName_Details()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.MarkComplete(It.IsAny<int>(), It.IsAny<Guid>());
            var actionResult = (RedirectToActionResult)result;
            Assert.Equal("Details", actionResult.ActionName);
        }

        [Fact]
        public void MarkConfirmedHasHttpPostAttribute()
        {
            var itineraryController = new ItineraryController(null, null, null);
            var httpPostAttribute = itineraryController.GetAttributesOn(x => x.MarkConfirmed(It.IsAny<int>(), It.IsAny<Guid>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(httpPostAttribute);
        }

        [Fact]
        public void MarkConfirmedHasRouteAttributeWithCorrectRoute()
        {
            var itineraryController = new ItineraryController(null, null, null);
            var routeAttribute = itineraryController.GetAttributesOn(x => x.MarkConfirmed(It.IsAny<int>(), It.IsAny<Guid>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/{itineraryId}/[Action]/{requestId}");
        }

        [Fact]
        public async Task MarkConfirmedSendsOrganizationIdQueryWithCorrectItineraryId()
        {
            var itineraryId = It.IsAny<int>();
            var mockMediator = new Mock<IMediator>();

            var itineraryController = new ItineraryController(mockMediator.Object, null, null);
            await itineraryController.MarkConfirmed(itineraryId, It.IsAny<Guid>());

            mockMediator.Verify(x => x.SendAsync(It.Is<OrganizationIdQuery>(y => y.ItineraryId == itineraryId)), Times.Once);
        }

        [Fact]
        public async Task MarkConfirmedReturnsHttpUnauthorizedWhenOrganizationIdIsZero()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(0);

            var itineraryController = new ItineraryController(mockMediator.Object, null, null);
            Assert.IsType<UnauthorizedResult>(await itineraryController.MarkConfirmed(It.IsAny<int>(), It.IsAny<Guid>()));
        }

        [Fact]
        public async Task MarkConfirmedReturnsHttpUnauthorizedWhenUserIsNotOrgAdmin()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var itineraryController = new ItineraryController(mockMediator.Object, null, null);
            itineraryController.MakeUserNotAnOrgAdmin();
            Assert.IsType<UnauthorizedResult>(await itineraryController.MarkConfirmed(It.IsAny<int>(), It.IsAny<Guid>()));
        }

        [Fact]
        public async Task MarkConfirmedSendsRequestStatusChangeCommandWithCorrectRequestIdWhenOrganizationIsNotZero_AndUserIsOrgAdmin()
        {
            var requestId = Guid.NewGuid();
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var itineraryController = new ItineraryController(mockMediator.Object, null, null);
            itineraryController.MakeUserAnOrgAdmin(orgId.ToString());

            await itineraryController.MarkConfirmed(It.IsAny<int>(), requestId);
            mockMediator.Verify(x => x.SendAsync(It.Is<ChangeRequestStatusCommand>(y => y.RequestId == requestId)), Times.Once);
        }

        [Fact]
        public async Task MarkConfirmedSendsRequestStatusChangeCommandWithCorrectStatusWhenOrganizationIsNotZero_AndUserIsOrgAdmin()
        {
            var requestId = Guid.NewGuid();
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var itineraryController = new ItineraryController(mockMediator.Object, null, null);
            itineraryController.MakeUserAnOrgAdmin(orgId.ToString());

            await itineraryController.MarkConfirmed(It.IsAny<int>(), requestId);
            mockMediator.Verify(x => x.SendAsync(It.Is<ChangeRequestStatusCommand>(y => y.NewStatus == RequestStatus.Confirmed)), Times.Once);
        }

        [Fact]
        public async Task MarkConfirmedReturnsRedirectToActionWhenOrganizationIsNotZero_AndUserIsOrgAdmin()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var itineraryController = new ItineraryController(mockMediator.Object, null, null);
            itineraryController.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await itineraryController.MarkConfirmed(It.IsAny<int>(), It.IsAny<Guid>());
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task MarkConfirmedReturnsRedirectToAction_WithActionName_Details()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var itineraryController = new ItineraryController(mockMediator.Object, null, null);
            itineraryController.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await itineraryController.MarkConfirmed(It.IsAny<int>(), It.IsAny<Guid>());
            var actionResult = (RedirectToActionResult)result;
            Assert.Equal("Details", actionResult.ActionName);
        }

        [Fact]
        public void MarkUnassignedHasHttpPostAttribute()
        {
            var itineraryController = new ItineraryController(null, null, null);
            var httpPostAttribute = itineraryController.GetAttributesOn(x => x.MarkUnassigned(It.IsAny<int>(), It.IsAny<Guid>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(httpPostAttribute);
        }

        [Fact]
        public void MarkUnassignedHasRouteAttributeWithCorrectRoute()
        {
            var itineraryController = new ItineraryController(null, null, null);
            var routeAttribute = itineraryController.GetAttributesOn(x => x.MarkUnassigned(It.IsAny<int>(), It.IsAny<Guid>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Itinerary/{itineraryId}/[Action]/{requestId}");
        }

        [Fact]
        public async Task MarkUnassignedSendsOrganizationIdQueryWithCorrectItineraryId()
        {
            var itineraryId = It.IsAny<int>();
            var mockMediator = new Mock<IMediator>();

            var itineraryController = new ItineraryController(mockMediator.Object, null, null);
            await itineraryController.MarkUnassigned(itineraryId, It.IsAny<Guid>());
            mockMediator.Verify(x => x.SendAsync(It.Is<OrganizationIdQuery>(y => y.ItineraryId == itineraryId)), Times.Once);
        }

        [Fact]
        public async Task MarkUnassignedReturnsHttpUnauthorizedWhenOrganizationIsZero()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(0);

            var itineraryController = new ItineraryController(mockMediator.Object, null, null);
            Assert.IsType<UnauthorizedResult>(await itineraryController.MarkUnassigned(It.IsAny<int>(), It.IsAny<Guid>()));
        }

        [Fact]
        public async Task MarkUnassignedReturnsHttpUnauthorizedWhenUserIsNotOrgAdmin()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var itineraryController = new ItineraryController(mockMediator.Object, null, null);
            itineraryController.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await itineraryController.MarkUnassigned(It.IsAny<int>(), It.IsAny<Guid>()));
        }

        [Fact]
        public async Task MarkUnassignedSendsRequestStatusChangeCommandWithCorrectRequestIdWhenOrganizationIsNotZero_AndUserIsOrgAdmin()
        {
            var requestId = Guid.NewGuid();
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var itineraryController = new ItineraryController(mockMediator.Object, null, null);
            itineraryController.MakeUserAnOrgAdmin(orgId.ToString());

            await itineraryController.MarkUnassigned(It.IsAny<int>(), requestId);
            mockMediator.Verify(x => x.SendAsync(It.Is<ChangeRequestStatusCommand>(y => y.RequestId == requestId)), Times.Once);
        }

        [Fact]
        public async Task MarkUnassignedSendsRequestStatusChangeCommandWithCorrectStatusWhenOrganizationIsNotZero_AndUserIsOrgAdmin()
        {
            var requestId = Guid.NewGuid();
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var itineraryController = new ItineraryController(mockMediator.Object, null, null);
            itineraryController.MakeUserAnOrgAdmin(orgId.ToString());

            await itineraryController.MarkUnassigned(It.IsAny<int>(), requestId);
            mockMediator.Verify(x => x.SendAsync(It.Is<ChangeRequestStatusCommand>(y => y.NewStatus == RequestStatus.Unassigned)), Times.Once);
        }

        [Fact]
        public async Task MarkUnassignedReturnsRedirectToActionWhenOrganizationIsNotZero_AndUserIsOrAdmin()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.MarkUnassigned(It.IsAny<int>(), It.IsAny<Guid>());
            Assert.IsType<RedirectToActionResult>(result);
        }


        [Fact]
        public async Task MarkUnassignedReturnsRedirectToAction_WithActionName_Details()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mockMediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.MarkUnassigned(It.IsAny<int>(), It.IsAny<Guid>());
            var actionResult = (RedirectToActionResult)result;
            Assert.Equal("Details", actionResult.ActionName);
        }

        [Fact]
        public async Task OptimizeRoute_ReturnsHttpUnauthorized_WhenUserIsNotOrgAdmin()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(1);

            var itineraryController = new ItineraryController(mockMediator.Object, null, new FakeUserManager());
            itineraryController.MakeUserNotAnOrgAdmin();

            Assert.IsType<UnauthorizedResult>(await itineraryController.OptimizeRoute(It.IsAny<int>(), It.IsAny<OptimizeRouteInputModel>()));
        }

        [Fact]
        public async Task OptimizeRoute_SendsOptimzeRouteCommand_Once_WhenUserIsOrgAdmin()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mockMediator.Object, null, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            await sut.OptimizeRoute(1, new OptimizeRouteInputModel { StartAddress = "123", EndSameAsStart = true });
            mockMediator.Verify(x => x.SendAsync(It.Is<OptimizeRouteCommand>(y => y.ItineraryId == 1 && y.UserId == "123")), Times.Once);
        }

        [Fact]
        public async Task OptimizeRoute_ReturnsRedirectToAction_WithCorrectRouteValues_WhenUserIsOrgAdmin()
        {
            const int orgId = 1;
            const int itineraryId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<OrganizationIdQuery>())).ReturnsAsync(orgId);

            var sut = new ItineraryController(mockMediator.Object, null, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.OptimizeRoute(itineraryId, new OptimizeRouteInputModel { StartAddress = "123", EndSameAsStart = true });

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            redirectResult.ActionName.ShouldBe("Details");

            var id = redirectResult.RouteValues["id"];
            id.ShouldNotBeNull();
            id.ShouldBe(itineraryId);
        }

        private class FakeUserManager : UserManager<ApplicationUser>
        {
            public FakeUserManager()
                : base(new Mock<IUserStore<ApplicationUser>>().Object,
                      new Mock<IOptions<IdentityOptions>>().Object,
                      new Mock<IPasswordHasher<ApplicationUser>>().Object,
                      new IUserValidator<ApplicationUser>[0],
                      new IPasswordValidator<ApplicationUser>[0],
                      new Mock<ILookupNormalizer>().Object,
                      new Mock<IdentityErrorDescriber>().Object,
                      new Mock<IServiceProvider>().Object,
                      new Mock<ILogger<UserManager<ApplicationUser>>>().Object)
            { }

            public override Task<ApplicationUser> GetUserAsync(ClaimsPrincipal user)
            {
                return Task.FromResult(new ApplicationUser { Id = "123" });
            }
        }

        private static Mock<IItineraryEditModelValidator> MockSuccessValidation()
        {
            var mockValidator = new Mock<IItineraryEditModelValidator>();
            mockValidator.Setup(mock => mock.Validate(It.IsAny<ItineraryEditViewModel>(), It.IsAny<EventSummaryViewModel>())).Returns(new List<KeyValuePair<string, string>>()).Verifiable();

            return mockValidator;
        }

        private ItineraryDetailsViewModel GetItineraryForSelectRequestHappyPathTests()
        {
            return new ItineraryDetailsViewModel
            {
                Id = 14,
                CampaignId = 123,
                CampaignName = "Campaign 1",
                EventId = 5412,
                Name = "Itinerary 3"
            };
        }

        private static void RunSelectRequestsHappyPathTests(IActionResult view, ItineraryDetailsViewModel itinerary, IList<RequestListViewModel> returnedRequests)
        {
            Assert.IsType<ViewResult>(view);

            var result = (ViewResult)view;
            Assert.IsType<SelectItineraryRequestsViewModel>(result.Model);

            var model = (SelectItineraryRequestsViewModel)result.Model;
            Assert.Equal(itinerary.CampaignId, model.CampaignId);
            Assert.Equal(itinerary.CampaignName, model.CampaignName);
            Assert.Equal(itinerary.EventId, model.EventId);
            Assert.Equal(itinerary.EventName, model.EventName);
            Assert.Equal(itinerary.Name, model.ItineraryName);

            Assert.Equal(returnedRequests.Count, model.Requests.Count);
            foreach (var request in returnedRequests)
            {
                var requestModel = model.Requests.FirstOrDefault(x => x.Id == request.Id);
                Assert.Equal(request.Name, requestModel.Name);
                Assert.Equal(request.DateAdded, requestModel.DateAdded);
                Assert.Equal(request.City, requestModel.City);
                Assert.Equal(request.Address, requestModel.Address);
                Assert.Equal(request.Latitude, requestModel.Latitude);
                Assert.Equal(request.Longitude, requestModel.Longitude);
                Assert.Equal(request.Postcode, requestModel.Postcode);
            }
        }

        private List<RequestListViewModel> GetRequestsForSelectRequestHappyPathTests()
        {
            return new List<RequestListViewModel>
            {
                new RequestListViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "asdfasf",
                    DateAdded = new DateTime(2016, 8, 7, 16, 9, 30),
                    City = "Wisconsin Dells",
                    Address = "123 Main St",
                    Latitude = 123.123123,
                    Longitude = -125.234,
                    Postcode = "53741"
                },
                new RequestListViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "asdfasf",
                    DateAdded = new DateTime(2015, 8, 7, 16, 9, 30),
                    City = "Springfield",
                    Address = "123 Main St",
                    Latitude = 38.123,
                    Longitude = -38.124,
                    Postcode = "12345"
                },
            };
        }
    }
}
