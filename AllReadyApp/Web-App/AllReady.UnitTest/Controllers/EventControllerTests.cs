using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Features.Event;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels.Event;
using AllReady.ViewModels.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class EventControllerTests
    {
        //delete this line when all unit tests using it have been completed
        private readonly Task taskFromResultZero = Task.FromResult(0);

        [Fact]
        public void IndexReturnsTheCorrectView()
        {
            var sut = EventControllerBuilder.Instance().Build();

            var result = sut.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void IndexHasHttpGetAttribute()
        {
            var sut = EventControllerBuilder.Instance().Build();

            var attribute = sut
                            .GetAttributesOn(x => x.Index())
                            .OfType<HttpGetAttribute>().FirstOrDefault();

            Assert.NotNull(attribute);
        }

        [Fact]
        public void ShowEventSendsShowEventQueryWithCorrectData()
        {

        }

        [Fact]
        public void ShowEventReturnsHttpNotFoundResultWhenViewModelIsNull()
        {
            var builder = EventControllerBuilder.Instance();
            var sut = builder.WithMediator().Build();

            builder
                .MediatorMock
                .Setup(x => x.Send(It.IsAny<ShowEventQuery>()))
                .Returns(() => null);

            var result = sut.ShowEvent(0);

            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void ShowEventReturnsEventWithTasksViewWithCorrrectViewModelWhenViewModelIsNotNull()
        {
            var builder = EventControllerBuilder.Instance();
            var eventViewModel = new EventViewModel();
            var sut = builder.WithMediator().Build();

            builder
                .MediatorMock
                .Setup(x => x.Send(It.IsAny<ShowEventQuery>()))
                .Returns(eventViewModel);

            var result = sut.ShowEvent(0) as ViewResult;

            Assert.Equal(eventViewModel, result.Model);
            Assert.Equal("EventWithTasks", result.ViewName);
        }

        [Fact]
        public void ShowEventHasRouteAttributeWithCorrectRoute()
        {
            var sut = EventControllerBuilder.Instance().Build();

            var attribute = sut
                            .GetAttributesOn(x => x.ShowEvent(0))
                            .OfType<RouteAttribute>().FirstOrDefault();

            Assert.NotNull(attribute);
            Assert.Equal("[controller]/{id}/", attribute.Template);
        }

        [Fact]
        public void ShowEventHasAllowAnonymousAttribute()
        {
            var sut = EventControllerBuilder.Instance().Build();

            var attribute = sut
                            .GetAttributesOn(x => x.ShowEvent(0))
                            .OfType<AllowAnonymousAttribute>().FirstOrDefault();

            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task SignupReturnsBadRequestResultWhenViewModelIsNull()
        {
            var sut = EventControllerBuilder.Instance().Build();

            var result = await sut.Signup(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task SignupSendsAsyncEventSignupCommandWithCorrrectDataWhenViewModelIsNotNull()
        {
            var builder = EventControllerBuilder.Instance();
            var sut = builder.WithMediator().Build();

            await sut.Signup(new EventSignupViewModel());

            builder
                .MediatorMock
                .Verify(x => x.SendAsync(It.IsAny<EventSignupCommand>()));
        }

        [Fact]
        public async Task SignupRedirectsToCorrectActionWithCorrectRouteValuesWhenViewModelIsNotNull()
        {
            var sut = EventControllerBuilder.Instance().WithMediator().Build();

            var result = await sut.Signup(new EventSignupViewModel()) as RedirectToActionResult;

            Assert.Equal(nameof(EventController.ShowEvent), result.ActionName);
            Assert.Equal(1, result.RouteValues.Count);
            Assert.Equal(0, result.RouteValues["id"]);
        }

        [Fact]
        public void SignupHasHttpPostAttribute()
        {
            var sut = EventControllerBuilder.Instance().Build();

            var attribute = sut
                            .GetAttributesOn(x => x.Signup(null))
                            .OfType<HttpPostAttribute>().FirstOrDefault();

            Assert.NotNull(attribute);
        }

        [Fact]
        public void SignupHasAuthorizeAttribute()
        {
            var sut = EventControllerBuilder.Instance().Build();

            var attribute = sut
                            .GetAttributesOn(x => x.Signup(null))
                            .OfType<AuthorizeAttribute>().FirstOrDefault();

            Assert.NotNull(attribute);
        }

        [Fact]
        public void SignupHasValidateAntiForgeryTokenAttribute()
        {
            var sut = EventControllerBuilder.Instance().Build();

            var attribute = sut
                            .GetAttributesOn(x => x.Signup(null))
                            .OfType<ValidateAntiForgeryTokenAttribute>().FirstOrDefault();

            Assert.NotNull(attribute);
        }

        [Fact]
        public void SignupHasRouteAttributeWithCorrectRoute()
        {
            var sut = EventControllerBuilder.Instance().Build();

            var attribute = sut
                            .GetAttributesOn(x => x.Signup(null))
                            .OfType<RouteAttribute>().FirstOrDefault();

            Assert.NotNull(attribute);
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeStatusReturnsBadRequestResultWhenUserIdIsNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeStatusSendsTaskStatusChangeCommandAsyncWithCorrectData()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeStatusRedirectsToCorrectActionWithCorrectRouteValues()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void ChangeStatusHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ChangeStatusHasRouteAttributeWithCorrectRoute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ChangeStatusHasAuthorizeAttribute()
        {
        }

        private class EventControllerBuilder
        {
            private IMediator _mediator;
            private ControllerContext _controllerContext;
            private static EventControllerBuilder _builder;
            private EventControllerBuilder()
            {
                MediatorMock = new Mock<IMediator>();
                _mediator = null;
            }

            public static EventControllerBuilder Instance()
            {
                if (_builder == null)
                    _builder = new EventControllerBuilder();

                return _builder;
            }

            public EventController Build()
            {
                if (_controllerContext == null)
                    return new EventController(_mediator, null);

                return new EventController(_mediator, null)
                {
                    ControllerContext = _controllerContext
                };
            }

            public Mock<IMediator> MediatorMock { get; }

            public EventControllerBuilder WithMediator()
            {

                _mediator = MediatorMock.Object;
                return this;
            }

            public EventControllerBuilder WithUserLogged()
            {
                var httpContext = new Mock<HttpContext>();

                var controllerContext = new Mock<ControllerContext>();
                var principal = new Moq.Mock<ClaimsPrincipal>();


                principal.Setup(p => p.IsInRole("Administrator")).Returns(true);
                principal.SetupGet(x => x.Identity.Name).Returns("userName");

                httpContext.Setup(x => x.User).Returns(principal.Object);
                controllerContext.Object.HttpContext = httpContext.Object;
                _controllerContext = controllerContext.Object;

                return this;
            }

        }

    }
}
