using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Features.Events;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels.Event;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        public async Task ShowEventSendsShowEventQueryWithCorrectData()
        {
            const string userId = "1";
            const int eventId = 1;

            var mediator = new Mock<IMediator>();

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser { Id = userId });
            
            var sut = new EventController(mediator.Object, userManager.Object);
            sut.SetFakeUser(userId);
            await sut.ShowEvent(eventId);

            mediator.Verify(x => x.SendAsync(It.Is<ShowEventQuery>(y => y.EventId == eventId)), Times.Once);
        }

        [Fact]
        public async Task ShowEventReturnsHttpNotFoundResultWhenViewModelIsNull()
        {
            const string userId = "1";

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ShowEventQuery>())).ReturnsAsync((EventViewModel)null);

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser { Id = userId });

            var sut = new EventController(mediator.Object, userManager.Object);
            sut.SetFakeUser(userId);

            var result = await sut.ShowEvent(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ShowEventReturnsCorrrectViewAndViewModelWhenViewModelIsNotNull()
        {
            const string userId = "1";

            var eventViewModel = new EventViewModel();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ShowEventQuery>())).ReturnsAsync(eventViewModel);

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser { Id = userId });

            var sut = new EventController(mediator.Object, userManager.Object);
            sut.SetFakeUser(userId);
            sut.SetFakeIUrlHelper();

            var result = await sut.ShowEvent(It.IsAny<int>()) as ViewResult;

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
            private readonly IMediator _mediator;
            private static EventControllerBuilder _builder;
            private EventControllerBuilder()
            {
                MediatorMock = new Mock<IMediator>();
                _mediator = null;
            }

            public static EventControllerBuilder Instance()
            {
                return _builder ?? (_builder = new EventControllerBuilder());
            }

            public EventController Build()
            {
                return new EventController(_mediator, null);
            }

            private Mock<IMediator> MediatorMock { get; }
        }
    }
}
