using System.Linq;
using AllReady.Controllers;
using AllReady.Models;
using AllReady.Services;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Moq;
using Xunit;
using MediatR;

namespace AllReady.UnitTest.Controllers
{
    public class AccountControllerTests
    {
        [Fact]
        public void LoginTestNullUrl()
        {
            var controller = AccountController();

            var result = (ViewResult) controller.Login();
            Assert.Null(result.ViewData["ReturnUrl"]);

            const string testUrl = "return url";
            result = (ViewResult)controller.Login(testUrl);
            Assert.Equal(testUrl, result.ViewData["ReturnUrl"]);
        }

        [Fact]
        public void LoginTestInvalidModel()
        {
            var controller = AccountController();
            controller.ModelState.AddModelError("foo", "bar");
            const string testUrl = "return url";
            var loginViewModel = new LoginViewModel();

            var result = controller.Login(loginViewModel, testUrl).GetAwaiter().GetResult();
            Assert.IsType<ViewResult>(result);
            var resultViewModel = ((ViewResult) result).ViewData.Model;
            Assert.IsType<LoginViewModel>(resultViewModel);
            Assert.Equal(resultViewModel, loginViewModel);
        }

        [Fact]
        public void LoginRedirectRemoteUrlTests()
        {
            var controller = AccountController();
            var loginViewModel = new LoginViewModel {Email = "", Password = "", RememberMe = false};

            const string testRemoteUrl = "http://foo.com/t";
            var result = controller.Login(loginViewModel, testRemoteUrl).GetAwaiter().GetResult();
            Assert.IsType<RedirectToActionResult>(result);
            var redirectToAction = (RedirectToActionResult)result;
            Assert.Equal("Home", redirectToAction.ControllerName);
            Assert.Equal(nameof(HomeController.Index), redirectToAction.ActionName);
        }

        [Fact]
        public void LoginRedirectLocalUrlTests()
        {
            var controller = AccountController();
            var loginViewModel = new LoginViewModel { Email = "", Password = "", RememberMe = false };

            const string testLocalUrl = "/foo/bar";
            var result = controller.Login(loginViewModel, testLocalUrl).GetAwaiter().GetResult();
            Assert.IsType<RedirectResult>(result);
            var redirectToLocalUrl = (RedirectResult)result;
            Assert.Equal(testLocalUrl, redirectToLocalUrl.Url);
        }

        [Fact]
        public void LoginFailureTests()
        {
            var controller = AccountController(SignInResult.Failed);
            var loginViewModel = new LoginViewModel { Email = "", Password = "", RememberMe = false };

            const string testLocalUrl = "/foo/bar";
            var result = controller.Login(loginViewModel, testLocalUrl).GetAwaiter().GetResult();

            Assert.IsType<ViewResult>(result);
            var resultViewModel = ((ViewResult)result).ViewData.Model;
            Assert.IsType<LoginViewModel>(resultViewModel);
            Assert.Equal(resultViewModel, loginViewModel);
            Assert.True(controller.ModelState[string.Empty].Errors
                .Select(x => x.ErrorMessage)
                .Contains("Invalid login attempt."));
        }

        private static AccountController AccountController(SignInResult signInResult = default(SignInResult))
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null,
                null, null);
            var mockHttpContext = new Mock<HttpContext>();

            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(mock => mock.HttpContext).Returns(() => mockHttpContext.Object);
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null, null);
            signInManagerMock.Setup(mock => mock
                .PasswordSignInAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(signInResult == default(SignInResult) 
                            ? SignInResult.Success 
                            : signInResult
                );
            var emailSenderMock = new Mock<IEmailSender>();
            var generalSettingsMock = new Mock<IOptions<GeneralSettings>>();
            var mediatorMock = new Mock<IMediator>();

            var controller = new AccountController(
                userManagerMock.Object,
                signInManagerMock.Object,
                emailSenderMock.Object,
                generalSettingsMock.Object,
                mediatorMock.Object);
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(mock => mock.IsLocalUrl(It.Is<string>(x => x.StartsWith("http"))))
                .Returns(false);
            urlHelperMock
                .Setup(mock => mock.IsLocalUrl(It.Is<string>(x => !x.StartsWith("http"))))
                .Returns(true);
            controller.Url = urlHelperMock.Object;
            return controller;
        }
    }
}
