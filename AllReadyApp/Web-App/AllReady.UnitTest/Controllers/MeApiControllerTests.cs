using System.Linq;
using AllReady.Controllers;
using AllReady.UnitTest.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class MeApiControllerTests
    {
        [Fact]
        public void IndexReturnsCorrectCookieString()
        {
            var sut = new MeApiController();
            var mockedHttpRequest = sut.GetMockHttpRequest();

            sut.Index();

            mockedHttpRequest.Verify(x => x.Cookies[".AspNet.ApplicationCookie"], Times.Once());
        }

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectRoute()
        {
            var sut = new MeApiController();
            var attribute = sut.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void IndexHasRouteAttributeWithRoute()
        {
            var sut = new MeApiController();
            var attribute = sut.GetAttributesOn(x => x.Index()).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "api/me");
        }
    }
}