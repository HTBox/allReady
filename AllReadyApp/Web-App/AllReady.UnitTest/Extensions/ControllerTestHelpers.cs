using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Mvc;
using Moq;

namespace AllReady.UnitTest.Extensions
{
    public static class ControllerTestHelpers
    {
        public static void SetDefaultHttpContext(this Controller controller)
        {
            controller.ActionContext.HttpContext = new DefaultHttpContext();
        }

        public static void SetFakeHttpRequestSchemeTo(this Controller controller, string requestScheme)
        {
            SetFakeHttpContextIfNotAlreadySet(controller);
            Mock.Get(controller.Request).SetupGet(httpRequest => httpRequest.Scheme).Returns(requestScheme);
        }

        public static T SetFakeUser<T>(this T controller, string userId) where T : Controller
        {
            SetFakeHttpContextIfNotAlreadySet(controller);

            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) }));

            Mock.Get(controller.HttpContext).SetupGet(httpContext => httpContext.User).Returns(claimsPrincipal);

            return controller;
        }

        public static void AddModelStateError(this Controller controller, string errorMessage)
        {
            controller.ViewData.ModelState.AddModelError("Error", errorMessage);
        }

        public static void AddModelStateError(this Controller controller)
        {
            controller.ViewData.ModelState.AddModelError("Error", "test");
        }

        public static void SetClaims(this Controller controller, List<Claim> claims)
        {
            SetFakeHttpContextIfNotAlreadySet(controller);

            var claimsIdentity = new ClaimsIdentity();
            claims.ForEach(claim => claimsIdentity.AddClaim(claim));
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            Mock.Get(controller.HttpContext).SetupGet(httpContext => httpContext.User).Returns(claimsPrincipal);
        }

        public static Mock<HttpContext> GetMockHttpContext(this Controller controller)
        {
            SetFakeHttpContextIfNotAlreadySet(controller);
            return Mock.Get(controller.HttpContext);
        }

        public static Mock<HttpRequest> GetMockHttpRequest(this Controller controller)
        {
            SetFakeHttpContextIfNotAlreadySet(controller);
            return Mock.Get(controller.Request);
        }

        public static Mock<HttpResponse> GetMockHttpResponse(this Controller controller)
        {
            SetFakeHttpContextIfNotAlreadySet(controller);
            return Mock.Get(controller.Response);
        }

        public static Mock<IUrlHelper> SetFakeIUrlHelper(this Controller controller)
        {
            controller.Url = new Mock<IUrlHelper>().Object;
            return Mock.Get(controller.Url);
        }

        private static void SetFakeHttpContextIfNotAlreadySet(Controller controller)
        {
            if (controller.ActionContext.HttpContext == null)
                controller.SetFakeHttpContext();
        }

        private static void SetFakeHttpContext(this Controller controller)
        {
            var httpContext = FakeHttpContext();
            controller.ActionContext.HttpContext = httpContext;
        }

        private static HttpContext FakeHttpContext()
        {
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();
            var mockHttpResponse = new Mock<HttpResponse>();

            mockHttpContext.SetupGet(httpContext => httpContext.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.SetupGet(httpContext => httpContext.Response).Returns(mockHttpResponse.Object);

            mockHttpRequest.Setup(httpRequest => httpRequest.Cookies[It.IsAny<string>()]).Returns(() => It.IsAny<string>());

            return mockHttpContext.Object;
        }
    }
}
