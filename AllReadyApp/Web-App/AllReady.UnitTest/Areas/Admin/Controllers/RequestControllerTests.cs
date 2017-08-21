using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Features.Sms;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
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
            Assert.Equal(attribute.Policy, null);
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

            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync((EventSummaryViewModel)null).Verifiable();

            var sut = new RequestController(mediator.Object);
            await sut.Create(id);

            mediator.Verify(x => x.SendAsync(It.Is<EventSummaryQuery>(a => a.EventId == id)), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenEventSummaryQueryReturnsNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync((EventSummaryViewModel)null).Verifiable();

            var sut = new RequestController(mediator.Object);
            var result = await sut.Create(1);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsForbidResult_WhenUserIsNotAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryViewModel());
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, false, false, false));

            var sut = new RequestController(mediator.Object);

            var result = await sut.Create(1);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCorrectViewAndViewModel_WhenEventIsNotNullAndUserIsAuthorized()
        {
            const int eventId = 1;
            const int orgId = 1;
            var viewModel = new EventSummaryViewModel { Id = eventId, OrganizationId = orgId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(viewModel);
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, false, false, true));

            var sut = new RequestController(mediator.Object);

            var result = await sut.Create(It.IsAny<int>()) as ViewResult;
            result.ViewName.ShouldBe("Edit");

            var resultViewModel = result.ViewData.Model as EditRequestViewModel;
            resultViewModel.EventId.ShouldBe(viewModel.Id);
            resultViewModel.OrganizationId.ShouldBe(viewModel.OrganizationId);

            var viewBagTitle = result.ViewData["Title"];
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
        public async Task EditPost_ReturnsViewResult_WhenModelStateIsNotValid()
        {
            const int eventId = 1;
            const int orgId = 1;
            var viewModel = new EventSummaryViewModel { Id = eventId, OrganizationId = orgId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(viewModel);

            var sut = new RequestController(mediator.Object);
            sut.MakeUserAnOrgAdmin(orgId.ToString());
            sut.ModelState.AddModelError("test", "test error");

            var result = await sut.Edit(new EditRequestViewModel { EventId = eventId }) as ViewResult;

            result.ViewName.ShouldBe("Edit");
        }

        [Fact]
        public async Task EditPost_SendsEventSummaryQuery_WithCorrectArguments()
        {
            var viewModel = new EditRequestViewModel { EventId = 1 };
            var mediator = new Mock<IMediator>();
            var sut = new RequestController(mediator.Object);
            await sut.Edit(viewModel);

            mediator.Verify(x => x.SendAsync(It.Is<EventSummaryQuery>(y => y.EventId == viewModel.EventId)));
        }

        [Fact]
        public async Task EditPost_ReturnsBadRequest_WhenEventSummaryQueryReturnsNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync((EventSummaryViewModel)null).Verifiable();

            var sut = new RequestController(mediator.Object);
            var result = await sut.Edit(new EditRequestViewModel { EventId = 1 });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task EditPost_ReturnsForbidResult_WhenUserIsNotAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryViewModel());
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, false, false, false));
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableRequestQuery>())).ReturnsAsync(new FakeAuthorizableRequest(false, false, false, false));

            var sut = new RequestController(mediator.Object);

            var result = await sut.Edit(new EditRequestViewModel { EventId = 1 });

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task EditPost_SendsValidatePhoneNumberRequestWithCorrectData_WhenModelStateIsValid_AndEventIsNotNull_AndUserIsAuthorized()
        {
            const int eventId = 1;
            const int orgId = 1;
            var model = new EditRequestViewModel { EventId = eventId, Phone = "111-111-1111" };

            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryViewModel { Id = eventId, OrganizationId = orgId });
            mediator.Setup(mock => mock.SendAsync(It.IsAny<ValidatePhoneNumberRequestCommand>())).ReturnsAsync(new ValidatePhoneNumberResult { IsValid = true, PhoneNumberE164 = model.Phone });
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, false, false, true));
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableRequestQuery>())).ReturnsAsync(new FakeAuthorizableRequest(false, true, false, false));

            var sut = new RequestController(mediator.Object);

            await sut.Edit(model);

            mediator.Verify(x => x.SendAsync(It.Is<ValidatePhoneNumberRequestCommand>(y => y.PhoneNumber == model.Phone && y.ValidateType)), Times.Once);
        }

        [Fact]
        public async Task EditPost_AddsCorrectErrorToModelState_WhenPhoneNumberValidationFails()
        {
            const int eventId = 1;
            const int orgId = 1;
            var model = new EditRequestViewModel { EventId = eventId, Phone = "111-111-1111" };

            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryViewModel { Id = eventId, OrganizationId = orgId });
            mediator.Setup(mock => mock.SendAsync(It.IsAny<ValidatePhoneNumberRequestCommand>())).ReturnsAsync(new ValidatePhoneNumberResult { IsValid = false });
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, false, false, true));
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableRequestQuery>())).ReturnsAsync(new FakeAuthorizableRequest(false, true, false, false));

            var sut = new RequestController(mediator.Object);

            var result = await sut.Edit(model) as ViewResult;
            Assert.Equal(result.ViewData.ModelState.ErrorCount, 1);
            Assert.Equal(result.ViewData.ModelState[nameof(model.Phone)].ValidationState, ModelValidationState.Invalid);
            Assert.Equal(result.ViewData.ModelState[nameof(model.Phone)].Errors.Single().ErrorMessage, "Unable to validate phone number");
        }

        [Fact]
        public async Task EditPost_ReturnsCorrectViewAndViewModel_WhenPhoneNumberValidationFails()
        {
            const int eventId = 1;
            const int orgId = 1;
            var model = new EditRequestViewModel { EventId = eventId, Phone = "111-111-1111" };

            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(new EventSummaryViewModel { Id = eventId, OrganizationId = orgId });
            mediator.Setup(mock => mock.SendAsync(It.IsAny<ValidatePhoneNumberRequestCommand>())).ReturnsAsync(new ValidatePhoneNumberResult { IsValid = false });
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, false, false, true));
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableRequestQuery>())).ReturnsAsync(new FakeAuthorizableRequest(false, true, false, false));

            var sut = new RequestController(mediator.Object);

            var result = await sut.Edit(model) as ViewResult;
            Assert.Equal(result.ViewName, "Edit");
            Assert.IsType<EditRequestViewModel>(result.Model);
        }

        [Fact]
        public async Task EditPost_SendsEditRequestCommandWithCorrrectModel_WhenModelStateIsValid_AndEventIsNotNull_AndUserIsAuthorized_AndPhoneNumberIsValid()
        {
            const int eventId = 1;
            const int orgId = 1;
            var viewModel = new EventSummaryViewModel { Id = eventId, OrganizationId = orgId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(viewModel);
            mediator.Setup(mock => mock.SendAsync(It.IsAny<ValidatePhoneNumberRequestCommand>())).ReturnsAsync(new ValidatePhoneNumberResult { IsValid = true });
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, false, false, true));
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableRequestQuery>())).ReturnsAsync(new FakeAuthorizableRequest(false, true, false, false));

            var sut = new RequestController(mediator.Object);

            var model = new EditRequestViewModel { EventId = eventId };

            await sut.Edit(model);

            mediator.Verify(x => x.SendAsync(It.Is<EditRequestCommand>(y => y.RequestModel == model)), Times.Once);
        }

        [Fact]
        public async Task EditPost_ReturnsRedirectToActionWithTheCorrectAcitonAndControllerName_WhenModelStateIsValid_AndEventIsNotNull_AndUserIsAuthorized_AndPhoneNumberIsValid()
        {
            const int eventId = 1;
            const int orgId = 1;
            var viewModel = new EventSummaryViewModel { Id = eventId, OrganizationId = orgId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<EventSummaryQuery>())).ReturnsAsync(viewModel);
            mediator.Setup(mock => mock.SendAsync(It.IsAny<ValidatePhoneNumberRequestCommand>())).ReturnsAsync(new ValidatePhoneNumberResult { IsValid = true });
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableEventQuery>())).ReturnsAsync(new FakeAuthorizableEvent(false, false, false, true));
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableRequestQuery>())).ReturnsAsync(new FakeAuthorizableRequest(false, true, false, false));

            var sut = new RequestController(mediator.Object);

            var result = await sut.Edit(new EditRequestViewModel { EventId = eventId }) as RedirectToActionResult;

            result.ActionName.ShouldBe("Requests");
            result.ControllerName.ShouldBe("Event");
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
        public async Task EditGet_SendsEditRequestQuery_WithCorrectArguments()
        {
            var eventId = Guid.NewGuid();
            var mediator = new Mock<IMediator>();

            var sut = new RequestController(mediator.Object);
            await sut.Edit(eventId);

            mediator.Verify(x => x.SendAsync(It.Is<EditRequestQuery>(y => y.Id == eventId)));
        }

        [Fact]
        public async Task EditGet_ReturnsNotFound_WhenEditRequestQueryReturnsNull()
        {
            var eventId = Guid.NewGuid();

            var mediator = new Mock<IMediator>();
            var sut = new RequestController(mediator.Object);

            var result = await sut.Edit(eventId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditGet_ReturnsForbidResult_WhenUserIsNotAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditRequestQuery>())).ReturnsAsync(new EditRequestViewModel());
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableRequestQuery>())).ReturnsAsync(new FakeAuthorizableRequest(false, false, false, false));

            var sut = new RequestController(mediator.Object);

            var result = await sut.Edit(Guid.NewGuid());

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task EditGet_ReturnsViewResult_WhenRequestIsFoundAndUserIsAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<EditRequestQuery>())).ReturnsAsync(new EditRequestViewModel { Name = "test" });
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableRequestQuery>())).ReturnsAsync(new FakeAuthorizableRequest(true, false, false, false));

            var sut = new RequestController(mediator.Object);

            var result = await sut.Edit(Guid.NewGuid()) as ViewResult;

            result.ViewName.ShouldBe("Edit");
            result.ViewData["Title"].ShouldBe("Edit test");
        }
    }
}
