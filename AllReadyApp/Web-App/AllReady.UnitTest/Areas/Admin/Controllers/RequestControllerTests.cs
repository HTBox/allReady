using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class RequestControllerTests
    {
        [Fact]
        public void ControllerHasAreaAtttribute_WithTheCorrectAreaName()
        {
            var sut = new RequestController(null);
            var attribute = sut.GetAttributes().OfType<AreaAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.RouteValue, "Admin");
        }

        [Fact]
        public void ControllerHasAreaAuthorizeAttribute_WithCorrectPolicy()
        {
            var sut = new RequestController(null);
            var attribute = sut.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Policy, "OrgAdmin");
        }

        [Fact]
        public void ControllerHasRouteAttribute_WithCorrectTemplate()
        {
            var sut = new RequestController(null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "Admin/Requests");
        }

        [Fact]
        public void Create_HasHttpGetAttribute()
        {
            var sut = new RequestController(null);
            var attribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void Create_HasRouteAttribute_WithCorrectTemplate()
        {
            var sut = new RequestController(null);
            var routeAttribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Create");
        }

        [Fact]
        public async Task Create_SendsEventSummaryQuery_WithCorrectEventId()
        {
            const int id = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(null).Verifiable();

            var sut = new RequestController(mockMediator.Object);
            await sut.Create(id);

            mockMediator.Verify(x => x.SendAsync(It.Is<EventSummaryQuery>(a => a.EventId == id)), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenEventSummaryQueryReturnsNull()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(null).Verifiable();

            var sut = new RequestController(mockMediator.Object);
            var result = await sut.Create(1);

            var objResult = Assert.IsType<BadRequestObjectResult>(result);
            objResult.StatusCode.ShouldNotBeNull();
            objResult.StatusCode.Value.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Create_ReturnsHttpUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryViewModel());

            var sut = new RequestController(mockMediator.Object);
            sut.MakeUserNotAnOrgAdmin();

            var result = await sut.Create(1);

            var objResult = Assert.IsType<UnauthorizedResult>(result);
            objResult.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task Create_ReturnsCorrectViewAndViewModel_WhenEventIsNotNullAndUserIsOrgAdmin()
        {
            const int eventId = 1;
            const int orgId = 1;
            var viewModel = new EventSummaryViewModel { Id = eventId, OrganizationId = orgId };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(viewModel);

            var sut = new RequestController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.Create(It.IsAny<int>());

            var objResult = Assert.IsType<ViewResult>(result);

            objResult.ViewName.ShouldBe("Edit");

            var resultViewModel = Assert.IsType<EditRequestViewModel>(objResult.ViewData.Model);
            resultViewModel.EventId.ShouldBe(viewModel.Id);
            resultViewModel.OrganizationId.ShouldBe(viewModel.OrganizationId);

            var viewBagTitle = objResult.ViewData["Title"];
            viewBagTitle.ShouldNotBeNull();
            viewBagTitle.ShouldBe(RequestController.CreateRequestTitle);
        }

        [Fact]
        public void EditPost_HasHttpPostAttribute()
        {
            var sut = new RequestController(null);
            var attribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<EditRequestViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void EditPost_HasRouteAttribute_WithCorrectTemplate()
        {
            var sut = new RequestController(null);
            var routeAttribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<EditRequestViewModel>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Edit");
            Assert.Equal(routeAttribute.Name, RequestController.EditRequestPostRouteName);
        }

        [Fact]
        public async Task EditPost_SendsEventSummaryQuery_WithCorrectArguments()
        {
            const int eventId = 1;

            EventSummaryQuery eventSummaryQuery = null; // will be assigned from the Moq callback
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(null).Callback<EventSummaryQuery>(cmd => eventSummaryQuery = cmd).Verifiable();

            var sut = new RequestController(mockMediator.Object);

            await sut.Edit(new EditRequestViewModel { EventId = eventId });

            mockMediator.Verify(x => x.SendAsync(It.IsAny<EventSummaryQuery>()), Times.Once);

            eventSummaryQuery.EventId.ShouldBe(eventId);
        }

        [Fact]
        public async Task EditPost_ReturnsBadRequest_WhenEventSummaryQueryReturnsNull()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(null).Verifiable();

            var sut = new RequestController(mockMediator.Object);
            var result = await sut.Edit(new EditRequestViewModel { EventId = 1 });

            var objResult = Assert.IsType<BadRequestObjectResult>(result);
            objResult.StatusCode.ShouldNotBeNull();
            objResult.StatusCode.Value.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task EditPost_ReturnsHttpUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryViewModel());

            var sut = new RequestController(mockMediator.Object);
            sut.MakeUserNotAnOrgAdmin();

            var result = await sut.Edit(new EditRequestViewModel { EventId = 1 });

            var objResult = Assert.IsType<UnauthorizedResult>(result);
            objResult.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task EditPost_ReturnsViewResult_WhenModelStateIsNotValid()
        {
            const int eventId = 1;
            const int orgId = 1;
            var viewModel = new EventSummaryViewModel { Id = eventId, OrganizationId = orgId };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(viewModel);

            var sut = new RequestController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(orgId.ToString());
            sut.ModelState.AddModelError("test", "test error");

            var result = await sut.Edit(new EditRequestViewModel { EventId = eventId });

            var objResult = Assert.IsType<ViewResult>(result);
            objResult.ViewName.ShouldBe("Edit");
        }

        [Fact]
        public async Task EditPost_SendsEditRequestCommand_WhenEventIsNotNullAndUserIsOrgAdmin()
        {
            const int eventId = 1;
            const int orgId = 1;
            var viewModel = new EventSummaryViewModel { Id = eventId, OrganizationId = orgId };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(viewModel);

            EditRequestCommand editRequestCommand = null; // will be assigned from the Moq callback
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EditRequestCommand>())).ReturnsAsync(Guid.NewGuid()).Callback<EditRequestCommand>(cmd => editRequestCommand = cmd).Verifiable();

            var sut = new RequestController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            await sut.Edit(new EditRequestViewModel { EventId = eventId });

            mockMediator.Verify(x => x.SendAsync(It.IsAny<EditRequestCommand>()), Times.Once);

            editRequestCommand.RequestModel.EventId.ShouldBe(eventId);
        }

        [Fact]
        public async Task EditPost_ReturnsRedirectToAction_WhenEventIsNotNullAndUserIsOrgAdmin()
        {
            const int eventId = 1;
            const int orgId = 1;
            var viewModel = new EventSummaryViewModel { Id = eventId, OrganizationId = orgId };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(viewModel);

            mockMediator.Setup(x => x.SendAsync(It.IsAny<EditRequestCommand>())).ReturnsAsync(Guid.NewGuid());

            var sut = new RequestController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.Edit(new EditRequestViewModel { EventId = eventId });

            var objResult = Assert.IsType<RedirectToActionResult>(result);
            objResult.ActionName.ShouldBe("Requests");
            objResult.ControllerName.ShouldBe("Event");
        }

        [Fact]
        public void EditGet_HasHttpGetAttribute()
        {
            var sut = new RequestController(null);
            var attribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<Guid>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void EditGet_HasRouteAttribute_WithCorrectTemplate()
        {
            var sut = new RequestController(null);
            var routeAttribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<Guid>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Edit/{id}");
        }

        [Fact]
        public async Task EditGet_SendsEditRequestQueryQuery_WithCorrectArguments()
        {
            var eventId = Guid.NewGuid();

            EditRequestQuery eventSummaryQuery = null; // will be assigned from the Moq callback
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EditRequestQuery>())).ReturnsAsync(null).Callback<EditRequestQuery>(cmd => eventSummaryQuery = cmd).Verifiable();

            var sut = new RequestController(mockMediator.Object);

            await sut.Edit(eventId);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<EditRequestQuery>()), Times.Once);

            eventSummaryQuery.Id.ShouldBe(eventId);
        }

        [Fact]
        public async Task EditGet_ReturnsNotFound_WhenEditRequestQueryReturnsNull()
        {
            var eventId = Guid.NewGuid();

            EditRequestQuery eventSummaryQuery = null; // will be assigned from the Moq callback
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EditRequestQuery>())).ReturnsAsync(null).Callback<EditRequestQuery>(cmd => eventSummaryQuery = cmd).Verifiable();

            var sut = new RequestController(mockMediator.Object);

            var result = await sut.Edit(eventId);

            var objResult = Assert.IsType<NotFoundResult>(result);
            objResult.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task EditGet_ReturnsHttpUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EditRequestQuery>())).ReturnsAsync(new EditRequestViewModel());

            var sut = new RequestController(mockMediator.Object);
            sut.MakeUserNotAnOrgAdmin();

            var result = await sut.Edit(Guid.NewGuid());

            var objResult = Assert.IsType<UnauthorizedResult>(result);
            objResult.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task EditGet_ReturnsViewResult_WhenRequestIsFoundAndUserIsOrgAdmin()
        {
            const int orgId = 1;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EditRequestQuery>())).ReturnsAsync(new EditRequestViewModel { OrganizationId = orgId, Name = "test" });

            var sut = new RequestController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(orgId.ToString());

            var result = await sut.Edit(Guid.NewGuid());

            var objResult = Assert.IsType<ViewResult>(result);
            objResult.ViewName.ShouldBe("Edit");
            objResult.ViewData["Title"].ShouldBe("Edit test");
        }
    }
}