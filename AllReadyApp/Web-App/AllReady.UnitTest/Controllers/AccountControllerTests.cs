using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Models;
using AllReady.Services;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class AccountControllerTests
    {
        [Fact]
        public void LoginTestNullUrl()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null, null);
            var mockHttpContext = new Mock<HttpContext>();

            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(mock => mock.HttpContext).Returns(() => mockHttpContext.Object);
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object, 
                contextAccessor.Object, 
                claimsFactory.Object, 
                null, null);
            var emailSenderMock = new Mock<IEmailSender>();
            var generalSettingsMock = new Mock<IOptions<GeneralSettings>>();

            var controller = new AccountController(
                userManagerMock.Object, 
                signInManagerMock.Object, 
                emailSenderMock.Object, 
                generalSettingsMock.Object);

            var result = (ViewResult) controller.Login();
            Assert.Null(result.ViewData["ReturnUrl"]);

            const string testUrl = "return url";
            result = (ViewResult)controller.Login(testUrl);
            Assert.Equal(testUrl, result.ViewData["ReturnUrl"]);
        }
    }
}
