using System.Linq;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.UnitTest.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class EventControllerTests
    {
        //delete this line when all unit tests using it have been completed
        private readonly Task taskFromResultZero = Task.FromResult(0);

        [Fact(Skip = "NotImplemented")]
        public void GetMyEventsSendsGetMyEventsQueryWithTheCorrectUserId()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void GetMyEventsReturnsTheCorrectViewAndViewModel()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void GetMyEventsHasRouteAttributeWithTheCorrectRoute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void GetMyEventsHasAuthorizeAttribute()
        {
        }

        

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
            
            var attribute = sut.GetAttributesOn(x => x.Index()).OfType<HttpGetAttribute>().FirstOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact(Skip = "NotImplemented")]
        public void ShowEventSendsShowEventQueryWithCorrectData()
        {

        }

        [Fact(Skip = "NotImplemented")]
        public void ShowEventReturnsHttpNotFoundResultWhenViewModelIsNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ShowEventReturnsEventWithTasksViewWithCorrrectViewModelWhenViewModelIsNotNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ShowEventHasRouteAttributeWithCorrectRoute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ShowEventHasAllowAnonymousAttribute()
        {
        }

        [Fact]
        public async Task SignupReturnsBadRequestResultWhenViewModelIsNull()
        {
            var sut = EventControllerBuilder.Instance().Build();
            var result = await sut.Signup(null);
            Assert.IsType<BadRequestResult>(result);

        }

        [Fact(Skip = "NotImplemented")]
        public async Task SignupSendsAsyncEventSignupCommandWithCorrrectDataWhenViewModelIsNotNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task SignupRedirectsToCorrectActionWithCorrectRouteValuesWhenViewModelIsNotNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact]
        public void SignupHasHttpPostAttribute()
        {
            var sut = EventControllerBuilder.Instance().Build();

            var attribute = sut.GetAttributesOn(x => x.Signup(null)).OfType<HttpPostAttribute>().FirstOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact(Skip = "NotImplemented")]
        public void SignupHasAuthorizeAttribute()
        {
            var sut = EventControllerBuilder.Instance().Build();

            var attribute = sut.GetAttributesOn(x => x.Signup(null)).OfType<AuthorizeAttribute>().FirstOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact(Skip = "NotImplemented")]
        public void SignupHasValidateAntiForgeryTokenAttribute()
        {
            var sut = EventControllerBuilder.Instance().Build();

            var attribute = sut.GetAttributesOn(x => x.Signup(null)).OfType<ValidateAntiForgeryTokenAttribute>().FirstOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact(Skip = "NotImplemented")]
        public void SignupHasRouteAttributeWithCorrectRoute()
        {
            var sut = EventControllerBuilder.Instance().Build();

            var attribute = sut.GetAttributesOn(x => x.Signup(null)).OfType<RouteAttribute>().FirstOrDefault();
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
            private readonly EventController _controller;

            private EventControllerBuilder()
            {
                _controller = new EventController(null,null);
            }

            public static EventControllerBuilder Instance()
            {
                return new EventControllerBuilder();
            }

            public EventController Build()
            {
                return _controller;
            }
        }
        
    }
}
