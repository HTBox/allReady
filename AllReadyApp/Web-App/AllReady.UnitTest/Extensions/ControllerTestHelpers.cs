using System;
using System.Collections.Generic;
using System.Security.Claims;
using AllReady.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Identity;

namespace AllReady.UnitTest.Extensions
{
    public static class ControllerTestHelpers
    {
        public static void SetDefaultHttpContext(this Controller controller)
        {
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        public static void SetFakeHttpRequestSchemeTo(this Controller controller, string requestScheme)
        {
            SetFakeHttpContextIfNotAlreadySet(controller);
            Mock.Get(controller.Request).SetupGet(httpRequest => httpRequest.Scheme).Returns(requestScheme);
        }

        public static void SetFakeUser(this Controller controller, string userId)
        {
            SetFakeHttpContextIfNotAlreadySet(controller);

            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) }));

            Mock.Get(controller.HttpContext).SetupGet(httpContext => httpContext.User).Returns(claimsPrincipal);
        }

        public static void SetFakeUserName(this Controller controller, string userName)
        {
            SetFakeHttpContextIfNotAlreadySet(controller);

            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Name, userName) }));

            Mock.Get(controller.HttpContext).SetupGet(httpContext => httpContext.User).Returns(claimsPrincipal);
        }

        public static void SetFakeUserWithCookieAuthenticationType(this Controller controller, string userId)
        {
            SetFakeHttpContextIfNotAlreadySet(controller);

            var identity = new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) }, IdentityConstants.ApplicationScheme);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            Mock.Get(controller.HttpContext).SetupGet(httpContext => httpContext.User).Returns(claimsPrincipal);
        }

        public static void SetFakeUserType(this Controller controller, UserType userType)
        {
            SetFakeHttpContextIfNotAlreadySet(controller);

            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof(UserType), userType))
            }));

            Mock.Get(controller.HttpContext).SetupGet(httpContext => httpContext.User).Returns(claimsPrincipal);
        }

        public static void SetFakeUserAndUserType(this Controller controller, string userId, UserType userType)
        {
            SetFakeHttpContextIfNotAlreadySet(controller);

            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(AllReady.Security.ClaimTypes.UserType, Enum.GetName(typeof (UserType), userType))
            }));

            Mock.Get(controller.HttpContext).SetupGet(httpContext => httpContext.User).Returns(claimsPrincipal);
        }

        public static void AddModelStateErrorWithErrorMessage(this Controller controller, string errorMessage)
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

        public static Mock<IUrlHelper> GetMockIUrlHelper(this Controller controller)
        {
            SetFakeIUrlHelperIfNotAlreadySet(controller);
            return Mock.Get(controller.Url);
        }

        private static void SetFakeIUrlHelperIfNotAlreadySet(Controller controller)
        {
            if (controller.Url == null)
                controller.SetFakeIUrlHelper();
        }

        public static Mock<IUrlHelper> SetFakeIUrlHelper(this Controller controller)
        {
            controller.Url = new Mock<IUrlHelper>().Object;
            return Mock.Get(controller.Url);
        }

        public static void MakeUserAnOrgAdmin(this Controller controller, string organizationId)
        {
            var orgAdminClaims = new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, organizationId)
            };

            SetFakeHttpContextIfNotAlreadySet(controller);
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(orgAdminClaims));
            Mock.Get(controller.HttpContext).SetupGet(httpContext => httpContext.User).Returns(claimsPrincipal);
        }

        public static void MakeUserASiteAdmin(this Controller controller)
        {
            var siteAdminClaim = new List<Claim> { new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.SiteAdmin)) };

            SetFakeHttpContextIfNotAlreadySet(controller);
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(siteAdminClaim));
            Mock.Get(controller.HttpContext).SetupGet(httpContext => httpContext.User).Returns(claimsPrincipal);
        }

        public static void MakeUserNotAnOrgAdmin(this Controller controller)
        {
            var claims = new List<Claim> { new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.BasicUser)) };

            SetFakeHttpContextIfNotAlreadySet(controller);
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            Mock.Get(controller.HttpContext).SetupGet(httpContext => httpContext.User).Returns(claimsPrincipal);
        }

        private static void SetFakeHttpContextIfNotAlreadySet(Controller controller)
        {
            if (controller.ControllerContext.HttpContext == null)
                controller.ControllerContext.HttpContext = FakeHttpContext();
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
