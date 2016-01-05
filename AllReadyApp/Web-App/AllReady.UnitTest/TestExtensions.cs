using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AllReady.UnitTest
{
    public static class TestExtensions
    {
        public static T WithUser<T>(this T controller, string userId)
            where T : Controller
        {
            var mockUser = new ClaimsPrincipal();
            var mockContext = new Mock<HttpContext>();
            mockUser.AddIdentity(
                new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId)
                    }
                )
            );
            mockContext.Setup(e => e.User).Returns(mockUser);
            controller.ActionContext.HttpContext = mockContext.Object;
            return controller;
        }
    }
}
