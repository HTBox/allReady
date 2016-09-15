using System.Linq;
using System.Security.Claims;
using AllReady.Controllers;
using AllReady.Features.Events;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels.Event;
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
            var builder = EventControllerBuilder.Instance();
            var sut = builder.WithMediator().WithUserLogged().Build();
            
            sut.ShowEvent(1);

            builder
                .MediatorMock
                .Verify(x => 
                    x.Send(It.Is<ShowEventQueryAsync>(y => 
                        y.EventId== 1 && y.User == sut.ControllerContext.HttpContext.User)));
        }

        [Fact]
        public void ShowEventReturnsHttpNotFoundResultWhenViewModelIsNull()
        {
            var builder = EventControllerBuilder.Instance();
            var sut = builder.WithMediator().Build();

            builder
                .MediatorMock
                .Setup(x => x.Send(It.IsAny<ShowEventQueryAsync>()))
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
                .Setup(x => x.Send(It.IsAny<ShowEventQueryAsync>()))
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
                    return new EventController(_mediator);

                return new EventController(_mediator)
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
